# Generated by Django 2.1.3 on 2018-11-25 09:22

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('router', '0014_truck_qr_code'),
    ]

    operations = [
        migrations.AlterField(
            model_name='edge',
            name='max_height',
            field=models.FloatField(default=9223372036854775807),
        ),
        migrations.AlterField(
            model_name='edge',
            name='max_weight',
            field=models.FloatField(default=9223372036854775807),
        ),
        migrations.AlterField(
            model_name='edge',
            name='max_width',
            field=models.FloatField(default=9223372036854775807),
        ),
        migrations.AlterField(
            model_name='truck',
            name='height',
            field=models.FloatField(default=0),
        ),
        migrations.AlterField(
            model_name='truck',
            name='weight',
            field=models.FloatField(default=0),
        ),
        migrations.AlterField(
            model_name='truck',
            name='width',
            field=models.FloatField(default=0),
        ),
    ]