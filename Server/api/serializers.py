from rest_framework import serializers

from router.models import Node, Edge


class NodeSerializer(serializers.ModelSerializer):
    class Meta:
        model = Node
        fields = (
            'id', 'longitude', 'latitude', 'adjacent_nodes',
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
            'id', 'node_1', 'node_2', 'distance', 'time', 'has_bus_stop'
        )