from django.shortcuts import render

# Create your views here.

import django_filters.rest_framework

from rest_framework import viewsets, permissions
from rest_framework.decorators import api_view, permission_classes
from rest_framework.response import Response
from rest_framework.filters import OrderingFilter

from api.serializers import NodeSerializer
from router.models import Node


class NodeViewSet(viewsets.ModelViewSet):
    """
    This viewset automatically provides `list` and `detail` actions.
    """
    permission_classes = (permissions.AllowAny,)
    filter_backends = (django_filters.rest_framework.DjangoFilterBackend, OrderingFilter)
    filter_fields = ('id',)
    queryset = Node.objects.all()
    serializer_class = NodeSerializer

