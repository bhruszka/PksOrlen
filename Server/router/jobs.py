from pksorlen.celery import app
from googlemaps.distance_matrix import distance_matrix
import googlemaps
from django.conf import settings


@app.task
def calculate_distances(node_id):
    from router.models import Node, Edge

    node = Node.objects.get(pk=node_id)

    gclient = googlemaps.Client(key=settings.GOOGLE_MAPS_API_KEY)

    if node.adjacent_nodes.exists():
        nodes = [n.get_node_tuple for n in node.adjacent_nodes.all()]

        distance_matrix_response = distance_matrix(gclient, node.get_node_tuple, nodes)
        assert distance_matrix_response['status'] == 'OK', 'Bad response from google maps API:\n{}'.format(distance_matrix_response)

        elements = distance_matrix_response['rows'][0]['elements']
        for i, n in enumerate(node.adjacent_nodes.all()):
            distance = elements[i]['distance']['value']
            duration = elements[i]['duration']['value']

            n1 = min(node, n, key=lambda x: x.id)
            n2 = max(node, n, key=lambda x: x.id)

            dist = Edge.objects.get_or_create(
                start_point=n1,
                end_point=n2,
            )
            dist.distance = distance
            dist.time = duration
            dist.save()

    node.all_distances_calculated = True
    node.save()
