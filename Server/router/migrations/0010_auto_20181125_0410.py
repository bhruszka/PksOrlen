# Generated by Django 2.1.3 on 2018-11-25 04:10

from django.db import migrations


class Migration(migrations.Migration):

    dependencies = [
        ('router', '0009_auto_20181125_0245'),
    ]

    operations = [
        migrations.AlterModelOptions(
            name='node',
            options={'verbose_name': 'Intersection', 'verbose_name_plural': 'Intersections'},
        ),
        migrations.RemoveField(
            model_name='node',
            name='all_distances_calculated',
        ),
    ]
