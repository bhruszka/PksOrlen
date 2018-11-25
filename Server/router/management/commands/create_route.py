import json
import logging
import random

from django.core.management import BaseCommand

from router.models import Node
from router.jobs import create_route
logger = logging.getLogger(__name__)


class Command(BaseCommand):
    def handle(self, *args, **kwargs):
        n1 = Node.objects.all()[0]
        n2 = Node.objects.all()[4]

        create_route(n1, n2)
