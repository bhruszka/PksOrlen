# Generated by Django 2.1.3 on 2018-11-25 02:45

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('router', '0008_node_is_gate'),
    ]

    operations = [
        migrations.AddField(
            model_name='edge',
            name='max_height',
            field=models.FloatField(null=True),
        ),
        migrations.AddField(
            model_name='edge',
            name='max_weight',
            field=models.FloatField(null=True),
        ),
        migrations.AddField(
            model_name='edge',
            name='max_width',
            field=models.FloatField(null=True),
        ),
        migrations.AddField(
            model_name='edge',
            name='open',
            field=models.BooleanField(default=True),
        ),
        migrations.AddField(
            model_name='node',
            name='turn_radius',
            field=models.FloatField(null=True),
        ),
    ]