from django.contrib import admin
from django.utils.safestring import mark_safe

from router.models import Node, Route, Edge, Truck


@admin.register(Node)
class NodeAdmin(admin.ModelAdmin):
    readonly_fields = ['longitude', 'latitude', 'adjacent_nodes']


@admin.register(Edge)
class EdgeAdmin(admin.ModelAdmin):
    readonly_fields = ['node_1', 'node_2', 'distance', 'time']


@admin.register(Route)
class RouteAdmin(admin.ModelAdmin):
    readonly_fields = ['route_ids_json', 'route']


@admin.register(Truck)
class TruckAdmin(admin.ModelAdmin):
    readonly_fields = ['start_node', 'end_node', 'start_edge', 'end_edge', 'qr_code', 'route']

    def qr_code(self, obj):
        return mark_safe('<img src="{url}" width="{width}" height={height} />'.format(
            url = obj.headshot.url,
            width=obj.headshot.width,
            height=obj.headshot.height,
            )
    )
