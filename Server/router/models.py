from django.db import models

# Create your models here.
from django.db.models.signals import post_save
from django.dispatch import receiver

from core.models import CommonModel
from router.jobs import calculate_distances


class Node(CommonModel):
    longitude = models.CharField(max_length=15)
    latitude = models.CharField(max_length=15)

    adjacent_nodes = models.ManyToManyField('self', blank=True, null=True)

    all_distances_calculated = models.BooleanField(default=False)

    @property
    def get_location_tuple(self):
        return self.latitude, self.longitude

    @staticmethod
    def create_from_string(val):
        lat, long = val.split(', ')
        n = Node(latitude=lat, longitude=long)
        n.save()
        return n

    @staticmethod
    def all_distances_ready():
        return not bool(Node.objects.filter(all_distances_calculated=False))

    class DistancesNotCalculatedException(Exception):
        pass


class Distance(CommonModel):
    node_1 = models.ForeignKey(Node, models.CASCADE, related_name='_start_distances')
    node_2 = models.ForeignKey(Node, models.CASCADE, related_name='_end_distances')

    distance = models.IntegerField()
    time = models.IntegerField()

    unique_together = ('node_1', 'node_2')

    def save(self, *args, **kwargs):
        if self.node_1.id > self.node_2.id:
            # to retain uniqueness in table require node_1 has lower id than node_2
            self.node_1, self.node_2 = self.node_2, self.node_1
        super(Distance, self).save(*args, **kwargs)

