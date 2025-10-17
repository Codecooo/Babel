DROP DATABASE IF EXISTS babel;
CREATE DATABASE babel;
\c babel;

-- Definisikan tipe enum

CREATE TYPE status_pesanan AS ENUM ('Menunggu Pembayaran', 'Pending', 'Selesai', 'Batal');
CREATE TYPE metode_pembayaran AS ENUM ('QRIS', 'Cash', 'Transfer');
CREATE TYPE status_mesin AS ENUM ('Online', 'Offline', 'Maintenance');
CREATE TYPE jabatan AS ENUM ('Print Operator', 'Banner Operator', 'DTF Operator', 'Customer Service');
CREATE TYPE status_pembayaran AS ENUM ('Menunggu Pembayaran', 'Berhasil', 'Batal');
CREATE TYPE jenis_mesin AS ENUM ('Printer A3+', 'Mesin DTF', 'Mesin Laminating', 'Printer A4', 'Mesin Banner');
CREATE TYPE jenis_produk AS ENUM ('Cetak', 'Laminating', 'Laminasi', 'Banner', 'DTF');
CREATE TYPE jenis_bahan AS ENUM ('HVS', 'Laminating', 'Laminasi', 'Banner', 'ID Card', 'DTF PET');
CREATE TYPE ukuran AS ENUM ('A4', 'A3', 'ID', 'A3+', '100+ CM');

-- Buat tabel

CREATE TABLE pelanggan (
  id_pelanggan   uuid            PRIMARY KEY,
  nama           text            NOT NULL,
  email          varchar(100)    NOT NULL,
  no_hp          varchar(20)     NOT NULL,
  alamat         text            NOT NULL
);

CREATE TABLE karyawan (
  id_karyawan    uuid            PRIMARY KEY,
  nama_depan     varchar(30)     NOT NULL,
  nama_belakang  varchar(30)     NOT NULL,
  jabatan        jabatan         NOT NULL,
  email          varchar(100)    NOT NULL,
  no_hp          varchar(20)     NOT NULL,
  alamat         text            NOT NULL
);

CREATE TABLE produk (
  id_produk         serial          PRIMARY KEY,
  ukuran            ukuran          NOT NULL,
  nama_produk       text            NOT NULL,
  jenis_produk      jenis_produk    NOT NULL,
  harga_per_unit    numeric(12,2)   NOT NULL
);

CREATE TABLE mesin (
  id_mesin       serial          PRIMARY KEY,
  nama_mesin     text            NOT NULL,
  status_mesin   status_mesin    NOT NULL,
  jenis_mesin    jenis_mesin     NOT NULL
);

CREATE TABLE produksi (
  id_produksi       serial      PRIMARY KEY,
  id_karyawan       uuid        NOT NULL,
  tanggal_produksi  date        NOT NULL,
  keterangan        text        NOT NULL,
  FOREIGN KEY       (id_karyawan) REFERENCES karyawan (id_karyawan)
);

CREATE TABLE pesanan (
  id_pesanan         uuid              PRIMARY KEY,
  id_pelanggan       uuid              NOT NULL,
  id_karyawan        uuid              NOT NULL,
  id_produksi        int               NOT NULL,
  tanggal_pesanan    timestamptz       NOT NULL,
  status_pesanan     status_pesanan    NOT NULL,
  FOREIGN KEY (id_pelanggan) REFERENCES pelanggan (id_pelanggan),
  FOREIGN KEY (id_karyawan) REFERENCES karyawan (id_karyawan),
  FOREIGN KEY (id_produksi) REFERENCES produksi (id_produksi)
);

CREATE TABLE pembayaran (
  id_pembayaran         uuid                PRIMARY KEY,
  id_pesanan            uuid                NOT NULL,
  metode_pembayaran     metode_pembayaran   NOT NULL,
  tanggal_pembayaran    timestamptz         NOT NULL,
  status_pembayaran     status_pembayaran   NOT NULL,
  total_pembayaran      numeric(12,2)       NOT NULL,
  FOREIGN KEY (id_pesanan) REFERENCES pesanan (id_pesanan) ON DELETE CASCADE
);

CREATE TABLE detail_pesanan (
  id_pesanan         uuid              NOT NULL,
  id_produk          int               NOT NULL,
  jumlah_produk      int               NOT NULL,
  FOREIGN KEY (id_pesanan) REFERENCES pesanan (id_pesanan) ON DELETE CASCADE,
  FOREIGN KEY (id_produk) REFERENCES produk (id_produk),
  PRIMARY KEY (id_pesanan, id_produk)
);

CREATE TABLE bahan_baku (
  id_bahan      serial      PRIMARY KEY,
  nama_bahan    text        NOT NULL,
  jenis_bahan   jenis_bahan NOT NULL,
  stok_bahan    int         NOT NULL
);

CREATE TABLE pemakaian_mesin (
  id_pesanan        uuid        NOT NULL,
  id_mesin          int         NOT NULL,
  waktu_pemakaian   timestamptz NOT NULL,
  FOREIGN KEY       (id_pesanan) REFERENCES pesanan (id_pesanan),
  FOREIGN KEY       (id_mesin) REFERENCES mesin (id_mesin),
  PRIMARY KEY (id_pesanan, id_mesin)
);

CREATE TABLE pemakaian_bahan (
  id_bahan      int        NOT NULL,
  id_produk     int        NOT NULL,
  FOREIGN KEY   (id_bahan) REFERENCES bahan_baku (id_bahan),
  FOREIGN KEY   (id_produk) REFERENCES produk (id_produk),
  PRIMARY KEY   (id_produk, id_bahan)
);