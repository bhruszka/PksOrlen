from pksorlen.celery import app
from googlemaps.distance_matrix import distance_matrix
import googlemaps
from django.conf import settings
import logging


logger = logging.getLogger(__name__)

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
        for i, n in enumerate(node.adjacent_nodes.filter(id__lt=node.id)):
            distance = elements[i]['distance']['value']
            duration = elements[i]['duration']['value']

            n1 = min(node, n, key=lambda x: x.id)
            n2 = max(node, n, key=lambda x: x.id)

            try:
                edge = Edge.objects.get(
                    node_1=n1,
                    node_2=n2,
                )
                edge.distance = distance
                edge.time = duration
            except Edge.DoesNotExist:
                edge = Edge(
                    node_1=n1,
                    node_2=n2,
                    distance=distance,
                    time=duration
                )
            edge.save()


    node.all_distances_calculated = True
    node.save()


@app.task
def create_route(initial_node_id, destination_node_id, truck_id=None):
    from router.models import Truck
    from router.models import Node, Edge, Route as RouteModel
    from router.utils.route_finder import get_feasible_nodes

    truck = Truck.objects.get(id=truck_id)

    initial_node = Node.objects.get(id=initial_node_id)
    destination_node = Node.objects.get(id=destination_node_id)

    from router.utils.route_finder import Route
    routes = [Route()]
    routes[0].append(initial_node)

    while not any(destination_node in route for route in routes):
        new_routes = []
        for route in routes:
            for node in get_feasible_nodes(route, truck):
                new_routes.append(route.copy_and_append(node))

        routes += new_routes

    first_route = None
    for route in routes:
        if destination_node in route:
            first_route = route
            break

    assert first_route is not None

    routes = [Route()]
    routes[0].append(first_route.path[-2])
    routes[0].append(first_route.path[-1])

    while not any(initial_node in route for route in routes):
        new_routes = []
        for route in routes:
            for node in get_feasible_nodes(route, truck):
                new_routes.append(route.copy_and_append(node))

        routes += new_routes

    second_route = None
    for route in routes:
        if initial_node in route:
            second_route = route
            break

    assert second_route is not None

    import json
    r = RouteModel.objects.create(
        route_ids_json=json.dumps([node.id for node in first_route.path + second_route.path[2:]])
    )

    Truck.objects.filter(id=truck_id).update(route=r)

