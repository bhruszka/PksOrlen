from django.contrib import admin

from router.models import Node, Route, Edge

admin.site.register(Node)
admin.site.register(Edge)
admin.site.register(Route)

