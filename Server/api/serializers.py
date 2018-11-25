from rest_framework import serializers

from router.models import Node, Edge, Truck, Route


class NodeSerializer(serializers.ModelSerializer):
    class Meta:
        model = Node
        fields = (
            'id', 'longitude', 'latitude', 'adjacent_nodes', 'is_gate', 'turn_radius'
        )
        ordering = ('id',)

    def to_internal_value(self, data):
        ret = super(NodeSerializer, self).to_internal_value(data)
        ret['id'] = data.get('id')
        return ret


class EdgeSerializer(serializers.ModelSerializer):
    node_1 = NodeSerializer()
    node_2 = NodeSerializer()

    class Meta:
        model = Edge
        fields = (
            'id', 'node_1', 'node_2', 'distance', 'time', 'has_bus_stop', 'max_height', 'max_width', 'open',
            'max_weight'
        )


class TruckSerializer(serializers.ModelSerializer):
    start_node = NodeSerializer()
    end_node = NodeSerializer()

    start_edge = EdgeSerializer()
    end_edge = EdgeSerializer()

    class Meta:
        model = Truck
        fields = (
            'id', 'height', 'width', 'weight', 'turn_radius', 'start_node', 'end_node', 'start_edge', 'end_edge',
        )


class RouteSerializer(serializers.ModelSerializer):
    route = NodeSerializer(many=True)

    class Meta:
        model = Route
        fields = (
            'id', 'route_ids_json', 'route',
        )
