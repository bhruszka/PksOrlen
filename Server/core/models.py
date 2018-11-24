from django.db import models
from django.contrib.auth.models import AbstractUser
from django.db.models.signals import post_save
from django.dispatch import receiver
from rest_framework.authtoken.models import Token


class CommonModel(models.Model):
    created_at = models.DateTimeField(auto_now_add=True)
    updated_at = models.DateTimeField(auto_now=True)

    class Meta:
        abstract = True


class CustomUser(AbstractUser):
    USERNAME_FIELD = 'username'
    facebook_id = models.IntegerField( null=True, blank=True)
    facebook_token = models.CharField(max_length=50, null=True, blank=True)
    avatar = models.ImageField('image', null=True, blank=True, upload_to='img')

    class Meta:
        verbose_name = 'user'
        verbose_name_plural = 'users'


@receiver(post_save, sender=CustomUser)
def create_auth_token(sender, instance=None, created=False, **kwargs):
    if created:
        Token.objects.create(user=instance)