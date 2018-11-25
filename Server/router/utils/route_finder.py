from pksorlen.celery import app
import json
import sys
from collections import defaultdict
from typing import List

from router.models import Node, Edge, Route as RouteModel


class Route:
    path = ...  # type: List[Node]

    def __init__(self):
        self.path = []

    def append(self, node: Node):
        self.path.append(node)

    def remove(self, node: Node):
        self.path.remove(node)

    def __iter__(self):
        return iter(self.path)

    def __repr__(self):
        return 'Route{}'.format(self.path)

    def __contains__(self, item):
        return item in self.path

    @property
    def distance(self):
        return sum(
            Edge.get_edge(n1, n2).distance
            for n1, n2 in zip(self.path, self.path[1:])
        )

    def copy_and_append(self, node: Node):
        copy = Route()
        copy.path = list(self.path)
        copy.append(node)
        return copy


def get_feasible_nodes(route: Route):
    return route.path[-1].adjacent_nodes.exclude(id__in=[n.id for n in route.path])



