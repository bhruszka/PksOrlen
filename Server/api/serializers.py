from rest_framework import serializers

from router.models import Node


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
