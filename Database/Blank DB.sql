PGDMP             	            x            JustOne    12.2    12.2                0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false                       0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false                       0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false                       1262    16393    JustOne    DATABASE        CREATE DATABASE "JustOne" WITH TEMPLATE = template0 ENCODING = 'UTF8' LC_COLLATE = 'English_United States.1252' LC_CTYPE = 'English_United States.1252';
    DROP DATABASE "JustOne";
                postgres    false                       0    0    DATABASE "JustOne"    ACL     *   GRANT ALL ON DATABASE "JustOne" TO ggweb;
                   postgres    false    2823                        2615    16460    JustOne    SCHEMA        CREATE SCHEMA "JustOne";
    DROP SCHEMA "JustOne";
                postgres    false            	           0    0    SCHEMA "JustOne"    ACL     (   GRANT ALL ON SCHEMA "JustOne" TO ggweb;
                   postgres    false    6            
           0    0    SCHEMA public    ACL     %   GRANT ALL ON SCHEMA public TO ggweb;
                   postgres    false    5            Λ            1259    16462    __EFMigrationsHistory    TABLE        CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);
 +   DROP TABLE public."__EFMigrationsHistory";
       public         heap    ggweb    false                      0    16462    __EFMigrationsHistory 
   TABLE DATA           R   COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
    public          ggweb    false    203   t       
           2606    16466 .   __EFMigrationsHistory PK___EFMigrationsHistory 
   CONSTRAINT     {   ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");
 \   ALTER TABLE ONLY public."__EFMigrationsHistory" DROP CONSTRAINT "PK___EFMigrationsHistory";
       public            ggweb    false    203                       826    16398     DEFAULT PRIVILEGES FOR SEQUENCES    DEFAULT ACL     M   ALTER DEFAULT PRIVILEGES FOR ROLE postgres GRANT ALL ON SEQUENCES  TO ggweb;
                   postgres    false                       826    16396     DEFAULT PRIVILEGES FOR FUNCTIONS    DEFAULT ACL     M   ALTER DEFAULT PRIVILEGES FOR ROLE postgres GRANT ALL ON FUNCTIONS  TO ggweb;
                   postgres    false                       826    16397    DEFAULT PRIVILEGES FOR TABLES    DEFAULT ACL     J   ALTER DEFAULT PRIVILEGES FOR ROLE postgres GRANT ALL ON TABLES  TO ggweb;
                   postgres    false                  xΡγββ Ε ©     