from rest_framework import serializers

from router.models import Node


class NodeSerializer(serializers.ModelSerializer):
    class Meta:
        model = Node
        fields = (
            'id', 'longitude', 'latitude', 'adjacent_nodes',
        )
        ordering = ('id',)
