-- Migration script to create providers table (Postgres)
CREATE TABLE IF NOT EXISTS providers (
    id serial PRIMARY KEY,
    name varchar(200) NOT NULL,
    latitude double precision NOT NULL,
    longitude double precision NOT NULL,
    isavailable boolean NOT NULL DEFAULT true,
    rating double precision NOT NULL DEFAULT 0,
    services text
);


INSERT INTO providers (name, latitude, longitude, isavailable, rating, services)
VALUES ('Proveedor Demo', 4.63, -74.06, true, 4.5, 'cerrageria, bateria');
INSERT INTO providers (name, latitude, longitude, isavailable, rating, services)
VALUES ('SERVIGRUAS', 5.23, -73.06, true, 4.0, 'Grua, batería'); 

CREATE TABLE IF NOT EXISTS optimizations (
    id serial PRIMARY KEY,
    requestname varchar(200) NOT NULL,
    latitude double precision NOT NULL,
    longitude double precision NOT NULL,
    rating double precision NOT NULL,
    providerid int NOT NULL REFERENCES providers(id)
);

