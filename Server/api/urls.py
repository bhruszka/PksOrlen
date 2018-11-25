from django.conf.urls import url
from django.urls import path, include
from rest_framework.routers import DefaultRouter
from api import views as api_views
from rest_framework.authtoken import views as token_views

# Create a router and register our viewsets with it.
router = DefaultRouter()
router.register(r'nodes', api_views.NodeViewSet)
router.register(r'edges', api_views.EdgeViewSet)

# The API URLs are now determined automatically by the router.
urlpatterns = [
    path('', include(router.urls)),
    url(r'^api-token-auth/', token_views.obtain_auth_token),
    url(r'^api-auth/', include('rest_framework.urls', namespace='rest_framework')),
    url(u'^generate-route/<int:start_node_id>/<int:destination_node_id>/', api_views.create_route),
    url(u'^nodes-bulk-patch/', api_views.bulk_patch_node),
]
