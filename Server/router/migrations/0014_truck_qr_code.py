# Generated by Django 2.1.3 on 2018-11-25 07:10

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('router', '0013_truck_route'),
    ]

    operations = [
        migrations.AddField(
            model_name='truck',
            name='qr_code',
            field=models.ImageField(blank=True, null=True, upload_to='qr_codes/'),
        ),
    ]
