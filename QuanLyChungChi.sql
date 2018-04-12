--To use Vietnamese,
----(1) choose the proper collation for Vietnam
------ALTER DATABASE your_db_name COLLATE Vietnamese_CI_AS
----(2) The column need to be supported the unicode character such as NVARCHAR, NCHAR, NTEXT.
----(3) Note the leading N before the string literal, telling SQL Server that this is a Unicode string literal
------N'Nguyễn Văn Hiệp'
----(4) Restart the computer.
CREATE TABLE w2s_test_type (
	test_type_id TINYINT IDENTITY(0,1) PRIMARY KEY,
	test_type_str CHAR(4),
);
INSERT INTO w2s_test_type VALUES ('EN_A'), ('EN_B'), ('EN_C'), ('IT_A'), ('IT_B');
CREATE TABLE w2s_examinee (
	test_date DATE,
	test_type_id TINYINT,
	examinee_index SMALLINT,
	name NVARCHAR(32) COLLATE Vietnamese_CI_AS,
	birth_date DATE,
	birth_place NVARCHAR(64) COLLATE Vietnamese_CI_AS,
	grade_1 FLOAT,
	grade_2 FLOAT,
	grade_3 FLOAT,
	PRIMARY KEY(test_date, test_type_id, examinee_index),
	FOREIGN KEY(test_type_id) REFERENCES w2s_test_type(test_type_id),
	--CHECK((3 < test_type_id AND grade_3 IS NULL) OR (test_type_id < 4 AND grade_3 IS NOT NULL))
);
DROP TABLE w2s_examinee;
DROP TABLE w2s_test_type;
Create table HoSo
(
	SoBaoDanh nvarchar(10) not null,
	Ngaythi date not null,
	HoVaTen nvarchar(100) not null,
	SinhNgay date not null,
	NoiSinh nvarchar(300) not null,
	----- Tin Hoc----
	DiemLyThuyet int not null,
	DiemThucHanh int not null,
	---- Anh Van----
	DiemNghe int not null,
	DiemDocViet int not null,
	DiemNoi int not null,
	----Ket qua----
	primary key (SoBaoDanh)
)--- TRUNG TÂM CÓ NHI?U KHÓA THI NÊN B?T BU?C PH?I CH?N NGÀY THI TRONG LÚC TÌM KI?M---
--1 Tìm theo tên ( chính xác: Trang, H?ng, Hung,...)
--2 Tìm theo h? (g?n dúng: Tr?n Anh, Tr?n, Nguy?n, Lê Th? Kim,...)
--3 Tim theo tên (g?n dúng: Tr,L,H?,...)

--4 Tìm t?t c? thí sinh theo ngày thi
--5 Tìm thì sinh theo so báo danh
--6 Tìm thí sinh theo tên và theo k?t qu? d?u - h?ng

-- 1 Tìm theo tên (Chính xác)
Select *from dbo.TTLA
where Ten like N'Trân%' and [ngày thi] like '%14%' and [NGÀY THI] like '%12%' and [NGÀY THI] like '%2008%' 

--2 Tìm theo h?(Nh?p h? nguy?n, tr?n nhung nh?p Tr, Nguy thì du?c)
Select *From dbo.TTLA
where [H? VÀ TÊN] like N'Nguy%' Select *from dbo.TTLA
where Ten like 'Trân%' and [ngày thi] like '%14%' and [NGÀY THI] like '%12%' and [NGÀY THI] like '%2008%' 

-- 3 Tìm theo tên g?n dúng
Select *From dbo.TTLA
where ten like 'H%' and [ngày thi] like '%14%' and [NGÀY THI] like '%12%' and [NGÀY THI] like '%2008%' 


--4 tìm theo ngày thi

Select *from dbo.TTLA
where  [ngày thi] like '%14%' and [NGÀY THI] like '%12%' and [NGÀY THI] like '%2008%' 
-- 5 Tìm theo s? báo danh
Select *from dbo.TTLA
where  SBD like 'b21%' and [NGÀY THI] like '%12%' and [NGÀY THI] like '%2008%' 

--6 Tìm thí sinh theo tên và theo k?t qu? d?u - h?ng

Select *from dbo.TTLA 
where ten like 'Trân%' and [K?T QU?]	like 'H%' and
[ngày thi] like '%14%' and [NGÀY THI] like '%12%' and [NGÀY THI] like '%2008%' 


----Tìm kiếm theo Tên và kết quả đậu hỏng----
Select *From dbo.benluc
Where Ten like '%T%' and [KẾT QUẢ] like '%H%'

