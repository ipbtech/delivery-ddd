using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"
                -- Пеший
                INSERT INTO public.couriers (id, name, speed, location_x, location_y)
                VALUES ('bf79a004-56d7-4e5f-a21c-0a9e5e08d10d', 'Пеший', 1, 1, 1)
                ON CONFLICT (id) DO NOTHING;

                INSERT INTO storage_places (id, name, order_id, total_volume, courier_id)
                VALUES ('ed58fa74-b8fb-4a8c-a84b-e5c29ca9b0c6', 'Сумка', NULL, 10, 'bf79a004-56d7-4e5f-a21c-0a9e5e08d10d')
                ON CONFLICT (id) DO NOTHING;

                -- Вело
                INSERT INTO public.couriers (id, name, speed, location_x, location_y)
                VALUES ('db18375d-59a7-49d1-bd96-a1738adcee93', 'Вело', 2, 2, 2)
                ON CONFLICT (id) DO NOTHING;

                INSERT INTO storage_places (id, name, order_id, total_volume, courier_id)
                VALUES 
                    ('b96a9d83-aefa-4d06-99fb-e630d17c3868', 'Вело-Сумка', NULL, 10, 'db18375d-59a7-49d1-bd96-a1738adcee93'),
                    ('838ac7aa-3f39-4b8a-b2be-f75fc3e35d34', 'Вело-Багажник', NULL, 30, 'db18375d-59a7-49d1-bd96-a1738adcee93')
                ON CONFLICT (id) DO NOTHING;

                -- Авто
                INSERT INTO public.couriers (id, name, speed, location_x, location_y)
                VALUES ('0f860f2c-d76a-4140-99b3-fcc63f27a826', 'Авто', 3, 3, 3)
                ON CONFLICT (id) DO NOTHING;

                INSERT INTO storage_places (id, name, order_id, total_volume, courier_id)
                VALUES 
                    ('f15b0f8c-dd93-4be6-a95a-3afd3a9f199e', 'Авто-Сумка', NULL, 10, '0f860f2c-d76a-4140-99b3-fcc63f27a826'),
                    ('84e1ccae-555d-439c-8c87-dae080c82d29', 'Авто-Багажник', NULL, 50, '0f860f2c-d76a-4140-99b3-fcc63f27a826'),
                    ('11fc6c0a-fc58-4718-b32d-8ce82e002201', 'Авто-Прицеп', NULL, 100, '0f860f2c-d76a-4140-99b3-fcc63f27a826')
                ON CONFLICT (id) DO NOTHING;
            ";

            migrationBuilder.Sql(sql);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}