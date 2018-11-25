from django.db import transaction
from django.shortcuts import render

# Create your views here.

import django_filters.rest_framework

from rest_framework import viewsets, permissions, status
from rest_framework.decorators import api_view, permission_classes
from rest_framework.response import Response
from rest_framework.filters import OrderingFilter
from rest_framework.views import APIView

from api.mixins import BulkyMethodsMixin
from api.serializers import NodeSerializer, EdgeSerializer, TruckSerializer, RouteSerializer
from router.models import Node, Edge, Truck, Route
from router.jobs import calculate_distances


class RouteViewSet(BulkyMethodsMixin, viewsets.ModelViewSet):
    # permission_classes = (permissions.AllowAny,)
    filter_backends = (django_filters.rest_framework.DjangoFilterBackend, OrderingFilter)
    filter_fields = ('id',)
    queryset = Route.objects.all()
    serializer_class = RouteSerializer


class TruckViewSet(BulkyMethodsMixin, viewsets.ModelViewSet):
    # permission_classes = (permissions.AllowAny,)
    filter_backends = (django_filters.rest_framework.DjangoFilterBackend, OrderingFilter)
    filter_fields = ('id',)
    queryset = Truck.objects.all()
    serializer_class = TruckSerializer


class EdgeViewSet(BulkyMethodsMixin, viewsets.ModelViewSet):
    # permission_classes = (permissions.AllowAny,)
    filter_backends = (django_filters.rest_framework.DjangoFilterBackend, OrderingFilter)
    filter_fields = ('id', 'has_bus_stop')
    queryset = Edge.objects.all()
    serializer_class = EdgeSerializer


class NodeViewSet(BulkyMethodsMixin, viewsets.ModelViewSet):
    """
    This viewset automatically provides `list` and `detail` actions.
    """
    # permission_classes = (permissions.AllowAny,)
    filter_backends = (django_filters.rest_framework.DjangoFilterBackend, OrderingFilter)
    filter_fields = ('id',)
    queryset = Node.objects.all()
    serializer_class = NodeSerializer

    def create(self, request, *args, **kwargs):
        if isinstance(request.data, list):
            Node.objects.all().delete()
        res = super(NodeViewSet, self).create(request, *args, **kwargs)
        if isinstance(request.data, list):
            for node in res.data:
                if node['adjacent_nodes']:
                    calculate_distances.delay(node['id'])
        else:
            if res.data['adjacent_nodes']:
                calculate_distances.delay(res.data['id'])

        return res

    def partial_update(self, request, *args, **kwargs):
        res = super(NodeViewSet, self).partial_update(request, *args, **kwargs)
        if isinstance(request.data, list):
            for node in res.data:
                if 'id' in res.data and ('longitude' in res.data or
                                         'latitude' in res.data or
                                         'adjacent_nodes' in res.data):
                    calculate_distances.delay(node['id'])
        else:
            if 'id' in res.data and ('longitude' in res.data or
                                     'latitude' in res.data or
                                     'adjacent_nodes' in res.data):
                calculate_distances.delay(res.data['id'])
        return res


@api_view(['PATCH'])
def bulk_patch_node(request):
    assert isinstance(request.data, list)

    for node in request.data:
        serializer = NodeSerializer(Node.objects.get(id=node['id']), data=node, partial=True)
        serializer.is_valid(raise_exception=True)
        serializer.save()
        if ('longitude' in node or
                'latitude' in node or
                'adjacent_nodes' in node):
            calculate_distances.delay(node['id'])

    return Response('OK')


@api_view(['POST'])
def create_truck_route(request):
    truck = Truck()
    if request.data['start']['type'] == 'node':
        truck.start_node = Node.objects.get(request.data['start']['id'])
    elif request.data['start']['type'] == 'edge':
        truck.start_edge = Edge.objects.get(request.data['start']['id'])

    if request.data['finish']['type'] == 'node':
        truck.end_node = Node.objects.get(request.data['start']['id'])
    elif request.data['finish']['type'] == 'edge':
        truck.end_edge = Edge.objects.get(request.data['start']['id'])

    truck.save()

    return Response(TruckSerializer(truck).data)


