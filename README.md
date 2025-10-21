# Babel Seeder
Program untuk membuat mock data ke database babel 

## Cara Penggunaan
1. Download programnya atau build sendiri
2. Buka terminal dan pindah ke direktori folder instalasi program Babel Seeder
   ``` bash
   cd ~/Downloads/Babel-Seeder
   ```
4. Untuk pertama kali jankan perintah untuk menginisialisasi database Babel sesuai schema
   ``` bash
   ./Babel init
   ```
5. Generate data mock agar dapat di load ke database menggunakan command ini
   ``` bash
   ./Babel generate (berapa banyak data)
   ```
6. Load data mock tadi dengan command
   ``` bash
   ./Babel load
   ```
7. Untuk menghapus semua data dalam semua tabel database gunakan EXPLODE
   ``` bash
   ./Babel explode
   ```
  
