from django.db import transaction
from django.shortcuts import render

# Create your views here.

import django_filters.rest_framework

from rest_framework import viewsets, permissions, status
from rest_framework.decorators import api_view, permission_classes
from rest_framework.response import Response
from rest_framework.filters import OrderingFilter

from api.serializers import NodeSerializer
from router.models import Node
from router.jobs import calculate_distances


class NodeViewSet(viewsets.ModelViewSet):
    """
    This viewset automatically provides `list` and `detail` actions.
    """
    # permission_classes = (permissions.AllowAny,)
    filter_backends = (django_filters.rest_framework.DjangoFilterBackend, OrderingFilter)
    filter_fields = ('id',)
    queryset = Node.objects.all()
    serializer_class = NodeSerializer

    def create(self, request, *args, **kwargs):
        res = super(NodeViewSet, self).create(request, *args, **kwargs)
        if 'id' in res.data and res.data['adjacent_nodes']:
            calculate_distances.delay(res.data['id'])
        return res

    def partial_update(self, request, *args, **kwargs):
        res = super(NodeViewSet, self).partial_update(request, *args, **kwargs)
        if 'id' in res.data and ('longitude' in res.data or
                                 'latitude' in res.data or
                                 'adjacent_nodes' in res.data):
            calculate_distances.delay(res.data['id'])
        return res


@api_view(['POST'])
def bulk_node_create(request):
    with transaction.atomic():
        Node.objects.all().delete()
        for node in request.data:
            serializer = NodeSerializer(data=node)
            serializer.save()
        return Response(status=status.HTTP_201_CREATED)

