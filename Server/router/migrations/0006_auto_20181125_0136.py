# Generated by Django 2.1.3 on 2018-11-25 01:36

from django.db import migrations


class Migration(migrations.Migration):

    dependencies = [
        ('router', '0005_auto_20181124_2130'),
    ]

    operations = [
        migrations.AlterUniqueTogether(
            name='edge',
            unique_together={('node_1', 'node_2')},
        ),
    ]