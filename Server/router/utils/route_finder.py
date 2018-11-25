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


def create_route(initial_node, destination_node):
    routes = [Route()]
    routes[0].append(initial_node)

    while not any(destination_node in route for route in routes):
        new_routes = []
        for route in routes:
            for node in get_feasible_nodes(route):
                new_routes.append(route.copy_and_append(node))

        routes += new_routes

    final_route = None
    for route in routes:
        if destination_node in route:
            final_route = route
            break

    assert final_route is not None

    return RouteModel.objects.create(
        route_ids_json=json.dumps([node.id for node in final_route.path])
    )


