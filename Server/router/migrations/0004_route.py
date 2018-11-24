# Generated by Django 2.1.3 on 2018-11-24 21:25

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('router', '0003_auto_20181124_2054'),
    ]

    operations = [
        migrations.CreateModel(
            name='Route',
            fields=[
                ('id', models.AutoField(auto_created=True, primary_key=True, serialize=False, verbose_name='ID')),
                ('created_at', models.DateTimeField(auto_now_add=True)),
                ('updated_at', models.DateTimeField(auto_now=True)),
                ('route_ids_json', models.TextField()),
            ],
            options={
                'abstract': False,
            },
        ),
    ]
