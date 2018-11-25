import json

from django.db import models

# Create your models here.
from django.db.models.signals import post_save
from django.dispatch import receiver
from django.db.models import Case, When

from core.models import CommonModel
from router.utils.route_finder import create_route


class Route(CommonModel):
    route_ids_json = models.TextField()

    @property
    def route_ids(self):
        return json.loads(self.route_ids_json)

    @property
    def route(self):
        """
        Return correctly ordered queryset of Nodes

        :return: Route Queryset
        """
        pk_list = self.route_ids
        preserved = Case(*[When(pk=pk, then=pos) for pos, pk in enumerate(pk_list)])
        return Node.objects.filter(pk__in=pk_list).order_by(preserved)


class Node(CommonModel):
    class Meta:
        verbose_name = 'Intersection'
        verbose_name_plural = 'Intersections'

    longitude = models.CharField(max_length=30)
    latitude = models.CharField(max_length=30)

    adjacent_nodes = models.ManyToManyField('self', blank=True)

    is_gate = models.BooleanField(default=False)

    turn_radius = models.FloatField(null=True)

    @property
    def get_node_tuple(self):
        return self.latitude, self.longitude

    @staticmethod
    def create_from_string(val):
        lat, long = val.split(', ')
        n = Node(latitude=lat, longitude=long)
        n.save()
        return n

    @staticmethod
    def all_distances_ready():
        return not bool(Node.objects.filter(all_distances_calculated=False))

    class DistancesNotCalculatedException(Exception):
        pass


class Edge(CommonModel):
    class Meta:
        verbose_name = 'Road'
        verbose_name_plural = 'Roads'

    node_1 = models.ForeignKey(Node, models.CASCADE, related_name='_start_edges')
    node_2 = models.ForeignKey(Node, models.CASCADE, related_name='_end_edges')

    distance = models.IntegerField()
    time = models.IntegerField()

    has_bus_stop = models.BooleanField(default=False)

    # road specific fields
    max_height = models.FloatField(null=True)
    max_width = models.FloatField(null=True)
    max_weight = models.FloatField(null=True)
    open = models.BooleanField(default=True)

    class Meta:
        unique_together = ('node_1', 'node_2')

    def save(self, *args, **kwargs):
        if self.node_1.id > self.node_2.id:
            # to retain uniqueness in table require node_1 has lower id than node_2
            self.node_1, self.node_2 = self.node_2, self.node_1
        super(Edge, self).save(*args, **kwargs)

    @staticmethod
    def get_edge(n1: Node, n2: Node):
        return Edge.objects.get(
            node_1=min(n1, n2, key=lambda x: x.id),
            node_2=max(n1, n2, key=lambda x: x.id),
        )


    @staticmethod
    def get_edge_ids(n1, n2):
        if n1 > n2:
            return Edge.objects.get(node_1=n2, node_2=n1)
        else:
            return Edge.objects.get(node_1=n1, node_2=n2)


class Truck(CommonModel):
    height = models.FloatField(null=True)
    width = models.FloatField(null=True)
    weight = models.FloatField(null=True)
    turn_radius = models.FloatField(null=True)

    start_node = models.ForeignKey(Node, models.CASCADE, null=True, related_name='trucks_starting')
    end_node = models.ForeignKey(Node, models.CASCADE, null=True, related_name='trucks_ending')

    start_edge = models.ForeignKey(Edge, models.CASCADE, null=True, related_name='trucks_starting')
    end_edge = models.ForeignKey(Edge, models.CASCADE, null=True, related_name='trucks_ending')

    route = models.ForeignKey(Route, models.SET_NULL, null=True, related_name='trucks')

    def save(self, *args, **kwargs):
        start = self.start_node or self.start_edge or None
        end = self.end_node or self.end_edge or None

        if start and end:
            start_node = None
            if isinstance(start, Edge):
                start_node = start.node_1
            else:
                start_node = start

            end_node = None
            if isinstance(end, Edge):
                end_node = end.node_1
            else:
                end_node = end

            self.route = create_route(start_node, end_node)

        super(Truck, self).save(*args, **kwargs)




