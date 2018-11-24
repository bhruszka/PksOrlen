class BulkyMethodsMixin:
    def get_serializer(self, *args, **kwargs):
        """Allows bulk creation of a resource."""
        if isinstance(kwargs.get('data', {}), list):
            kwargs['many'] = True

        return super().get_serializer(*args, **kwargs)
