DECLARE @FirstName NVARCHAR(50) = 'Nguyễn';  -- Ví dụ tên
DECLARE @LastName NVARCHAR(50) = 'Văn A';    -- Ví dụ họ
DECLARE @Phone NVARCHAR(20) = CONCAT('090', CAST(ABS(CHECKSUM(NEWID())) % 1000000000 AS NVARCHAR(10)));  -- Số điện thoại ngẫu nhiên
DECLARE @Email NVARCHAR(100) = CONCAT(@FirstName, @LastName, '@example.com');  -- Email

DECLARE @IsActive INT = 1;  -- Trạng thái hoạt động

-- Thêm bản ghi vào bảng Owner
INSERT INTO dbo.Owner (FirstName, LastName, Phone, Email, Username, [Password], IsActive)
VALUES (@FirstName, @LastName, @Phone, @Email, 'owner', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', @IsActive);
GO
INSERT INTO [dbo].[Staff] (OwnerId, FirstName, LastName, Phone, Email, Username, [Password], IsActive) VALUES
(1, N'Tuấn', N'Đỗ', '0997980090', 'tuan.do@zalo.vn', 'tuan.do1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Duy', N'Đỗ', '0923260793', 'duy.do@yahoo.com', 'duy.do1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Phong', N'Lê', '0961043969', 'phong.le@outlook.com', 'phong.le1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Trang', N'Đặng', '0925157188', 'trang.dang@outlook.com', 'trang.dang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Linh', N'Vũ', '0936987271', 'linh.vu@yahoo.com', 'linh.vu1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Tuấn', N'Đặng', '0928240487', 'tuan.dang@yahoo.com', 'tuan.dang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Quang', N'Trần', '0927690763', 'quang.tran@zalo.vn', 'quang.tran1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Tuấn', N'Đặng', '0997749747', 'tuan.dang2@yahoo.com', 'tuan.dang2', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'An', N'Vũ', '0993407191', 'an.vu@yahoo.com', 'an.vu1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Quang', N'Hồ', '0933377204', 'quang.ho@zalo.vn', 'quang.ho1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Linh', N'Vũ', '0918973891', 'linh.vu2@gmail.com', 'linh.vu2', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Minh', N'Đặng', '0941357036', 'minh.dang@outlook.com', 'minh.dang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Minh', N'Đỗ', '0951214911', 'minh.do@yahoo.com', 'minh.do1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Quang', N'Bùi', '0968529303', 'quang.bui@outlook.com', 'quang.bui1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'An', N'Phạm', '0931679589', 'an.pham@zalo.vn', 'an.pham1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Trang', N'Phạm', '0964715583', 'trang.pham@outlook.com', 'trang.pham1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Hải', N'Bùi', '0991860680', 'hai.bui@gmail.com', 'hai.bui1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Linh', N'Đặng', '0921554935', 'linh.dang@gmail.com', 'linh.dang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Linh', N'Hoàng', '0993201151', 'linh.hoang@gmail.com', 'linh.hoang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Phong', N'Nguyễn', '0918169747', 'phong.nguyen@gmail.com', 'phong.nguyen1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0);

GO
INSERT INTO [dbo].[Customer] (OwnerId, FirstName, LastName, Phone, Email, Username, Password, IsActive) VALUES
(1, N'Hải', N'Vũ', '0974153910', 'hai.vu@gmail.com', 'hai.vu1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Minh', N'Đặng', '0935590898', 'minh.dang@zalo.vn', 'minh.dang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Linh', N'Hồ', '0914198004', 'linh.ho@zalo.vn', 'linh.ho1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Hải', N'Phạm', '0913901532', 'hai.pham@yahoo.com', 'hai.pham1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Hương', N'Đỗ', '0941976341', 'huong.do1@yahoo.com', 'huong.do1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Hương', N'Đỗ', '0986045074', 'huong.do2@yahoo.com', 'huong.do2', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Linh', N'Hoàng', '0959403980', 'linh.hoang@outlook.com', 'linh.hoang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Phong', N'Đặng', '0928420764', 'phong.dang@zalo.vn', 'phong.dang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'An', N'Hoàng', '0989254406', 'an.hoang@yahoo.com', 'an.hoang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Tuấn', N'Đặng', '0987358718', 'tuan.dang1@yahoo.com', 'tuan.dang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'An', N'Trần', '0978285027', 'an.tran@yahoo.com', 'an.tran1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Phong', N'Bùi', '0918562015', 'phong.bui@outlook.com', 'phong.bui1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Duy', N'Vũ', '0994398738', 'duy.vu@zalo.vn', 'duy.vu1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Minh', N'Đỗ', '0932466311', 'minh.do@outlook.com', 'minh.do1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Tuấn', N'Nguyễn', '0931693316', 'tuan.nguyen1@yahoo.com', 'tuan.nguyen1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Hải', N'Hồ', '0953812026', 'hai.ho@gmail.com', 'hai.ho1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Tuấn', N'Lê', '0962809916', 'tuan.le@zalo.vn', 'tuan.le1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'An', N'Đặng', '0979704002', 'an.dang@zalo.vn', 'an.dang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Tuấn', N'Đặng', '0967170761', 'tuan.dang2@yahoo.com', 'tuan.dang2', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Hải', N'Hoàng', '0935622622', 'hai.hoang@yahoo.com', 'hai.hoang1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Tuấn', N'Đặng', '0974760653', 'tuan.dang3@yahoo.com', 'tuan.dang3', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Tuấn', N'Nguyễn', '0974744625', 'tuan.nguyen2@gmail.com', 'tuan.nguyen2', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Duy', N'Trần', '0912424120', 'duy.tran@gmail.com', 'duy.tran1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0),
(1, N'Hương', N'Nguyễn', '0965130827', 'huong.nguyen@zalo.vn', 'huong.nguyen1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 1),
(1, N'Duy', N'Hồ', '0970195649', 'duy.ho@outlook.com', 'duy.ho1', 'f3ed11bbdb94fd9ebdefbaf646ab94d3', 0);
GO
INSERT INTO [dbo].[Car] (CustomerId, Model, Color, LicensePlate, RegistedDate, Status) VALUES
(1, N'Mazda 3', N'White', '49A-62335', '2023-11-09 14:31:31', 1),
(1, N'Kia Forte', N'White', '59A-84037', '2023-02-15 14:31:31', 0),
(2, N'Toyota Corolla', N'Blue', '62A-33995', '2023-06-07 14:31:31', 0),
(3, N'Ford Focus', N'Red', '35A-81060', '2024-11-26 14:31:31', 1),
(4, N'Ford Focus', N'Black', '85A-85502', '2023-05-24 14:31:31', 1),
(5, N'Honda Civic', N'Gray', '60A-90518', '2024-09-10 14:31:31', 0),
(5, N'Honda Civic', N'Silver', '28A-71341', '2024-02-17 14:31:31', 0),
(6, N'Honda Civic', N'Red', '34A-10503', '2023-03-08 14:31:31', 0),
(7, N'Mazda 3', N'Red', '42A-84568', '2023-08-20 14:31:31', 0),
(7, N'Hyundai Elantra', N'Black', '69A-27729', '2023-03-25 14:31:31', 1),
(8, N'Ford Focus', N'Gray', '61A-64046', '2023-01-17 14:31:31', 1),
(8, N'Kia Forte', N'Red', '35A-82563', '2024-12-04 14:31:31', 0),
(9, N'Toyota Corolla', N'Silver', '61A-26058', '2023-03-10 14:31:31', 1),
(9, N'Hyundai Elantra', N'Gray', '99A-25660', '2023-09-20 14:31:31', 0),
(10, N'Ford Focus', N'Black', '11A-28447', '2023-01-13 14:31:31', 0),
(11, N'Ford Focus', N'Red', '90A-23767', '2024-02-19 14:31:31', 1),
(12, N'Hyundai Elantra', N'Blue', '38A-93542', '2022-07-30 14:31:31', 0),
(13, N'Ford Focus', N'Black', '52A-82132', '2022-11-27 14:31:31', 0),
(13, N'Ford Focus', N'Silver', '35A-13104', '2024-01-22 14:31:31', 1),
(14, N'Ford Focus', N'Red', '25A-51186', '2023-06-27 14:31:31', 0),
(14, N'Hyundai Elantra', N'Red', '54A-62978', '2023-11-24 14:31:31', 1),
(15, N'Kia Forte', N'Silver', '30A-58637', '2024-03-11 14:31:31', 0),
(1, N'Kia Forte', N'Red', '99A-33742', '2023-05-08 14:32:40', 0),
(2, N'Honda Civic', N'Silver', '86A-50211', '2024-01-31 14:32:40', 0),
(3, N'Ford Focus', N'Red', '49A-29093', '2024-11-03 14:32:40', 0),
(3, N'Mazda 3', N'White', '82A-54044', '2022-10-29 14:32:40', 1),
(4, N'Honda Civic', N'Blue', '22A-13134', '2023-03-25 14:32:40', 0),
(4, N'Honda Civic', N'Silver', '79A-42043', '2022-12-02 14:32:40', 1),
(5, N'Ford Focus', N'Blue', '53A-90918', '2024-07-10 14:32:40', 1),
(5, N'Toyota Corolla', N'Red', '74A-30746', '2022-07-02 14:32:40', 0),
(5, N'Ford Focus', N'Red', '31A-32550', '2022-11-13 14:32:40', 1),
(6, N'Mazda 3', N'Silver', '71A-26842', '2023-04-14 14:32:40', 1),
(6, N'Mazda 3', N'Silver', '96A-71983', '2024-01-14 14:32:40', 0),
(6, N'Toyota Corolla', N'Red', '60A-57584', '2023-02-13 14:32:40', 1),
(7, N'Honda Civic', N'White', '80A-20360', '2023-06-16 14:32:40', 0),
(8, N'Honda Civic', N'Black', '25A-18290', '2023-12-27 14:32:40', 1),
(8, N'Toyota Corolla', N'Silver', '83A-35292', '2024-10-25 14:32:40', 1),
(9, N'Honda Civic', N'Black', '63A-57154', '2024-03-08 14:32:40', 1),
(9, N'Honda Civic', N'Red', '56A-13719', '2022-06-30 14:32:40', 0),
(9, N'Mazda 3', N'Red', '33A-21592', '2023-05-17 14:32:40', 1),
(10, N'Ford Focus', N'Gray', '23A-58529', '2023-11-25 14:32:40', 0),
(11, N'Ford Focus', N'Red', '33A-31395', '2024-05-13 14:32:40', 1),
(11, N'Honda Civic', N'Gray', '58A-39891', '2023-12-05 14:32:40', 1),
(12, N'Toyota Corolla', N'Blue', '73A-97145', '2022-10-22 14:32:40', 1),
(12, N'Honda Civic', N'Gray', '52A-50812', '2023-06-28 14:32:40', 0),
(12, N'Kia Forte', N'White', '14A-97286', '2023-09-18 14:32:40', 0),
(13, N'Toyota Corolla', N'Red', '16A-29342', '2024-04-02 14:32:40', 0),
(13, N'Toyota Corolla', N'Gray', '18A-15738', '2023-09-20 14:32:40', 0),
(13, N'Honda Civic', N'Black', '48A-35763', '2024-03-29 14:32:40', 1),
(14, N'Honda Civic', N'Black', '23A-59773', '2023-12-04 14:32:40', 0),
(14, N'Ford Focus', N'Gray', '76A-70656', '2023-07-27 14:32:40', 1),
(15, N'Kia Forte', N'Black', '77A-40919', '2023-01-25 14:32:40', 1),
(16, N'Mazda 3', N'Gray', '80A-37106', '2024-04-13 14:32:40', 1),
(17, N'Mazda 3', N'Gray', '88A-46784', '2023-07-21 14:32:40', 1),
(18, N'Honda Civic', N'White', '35A-73852', '2022-07-18 14:32:40', 0),
(18, N'Hyundai Elantra', N'Black', '52A-50821', '2023-07-21 14:32:40', 0),
(18, N'Honda Civic', N'Silver', '13A-34420', '2022-09-16 14:32:40', 1),
(19, N'Mazda 3', N'Red', '65A-44018', '2022-10-03 14:32:40', 0),
(19, N'Ford Focus', N'Red', '59A-62824', '2022-11-28 14:32:40', 1),
(19, N'Mazda 3', N'Red', '44A-93032', '2024-04-19 14:32:40', 0),
(20, N'Hyundai Elantra', N'Blue', '30A-83722', '2024-01-02 14:32:40', 0),
(20, N'Toyota Corolla', N'Red', '12A-33386', '2024-05-27 14:32:40', 1),
(21, N'Hyundai Elantra', N'Blue', '12A-79258', '2024-10-15 14:32:40', 0),
(22, N'Toyota Corolla', N'Blue', '53A-41410', '2023-12-18 14:32:40', 1),
(22, N'Toyota Corolla', N'Blue', '93A-18532', '2022-10-08 14:32:40', 0),
(23, N'Ford Focus', N'Gray', '13A-35878', '2023-08-27 14:32:40', 0),
(23, N'Ford Focus', N'Silver', '67A-81228', '2023-01-13 14:32:40', 0),
(24, N'Toyota Corolla', N'White', '62A-72784', '2023-01-05 14:32:40', 1),
(25, N'Honda Civic', N'Gray', '73A-55306', '2023-01-12 14:32:40', 1),
(25, N'Kia Forte', N'Black', '49A-26990', '2024-05-17 14:32:40', 1);
GO
INSERT INTO dbo.ParkingLot (OwnerId, PricePerHour, PricePerDay, PricePerMonth, Address, [Long], Lat, CreatedDate, UpdatedDate) VALUES
(1, 3.50, 30.00, 300.00, N'123 Lê Duẩn, Phường Bến Nghé, Quận 1, TP.HCM', 106.703722, 10.775843, '2025‑03‑01 08:00:00', '2025‑03‑01 08:00:00'),
(1, 4.00, 35.00, 350.00, N'456 Nguyễn Thị Minh Khai, Phường 5, Quận 3, TP.HCM', 106.696557, 10.782778, '2025‑03‑02 09:30:00', '2025‑03‑02 09:30:00'),
(1, 2.50, 25.00, 250.00, N'789 Cách Mạng Tháng 8, Phường 10, Quận 3, TP.HCM', 106.685319, 10.779404, '2025‑03‑03 10:15:00', '2025‑03‑03 10:15:00'),
(1, 5.00, 40.00, 400.00, N'101 Trần Hưng Đạo, Phường Nguyễn Cư Trinh, Quận 1, TP.HCM', 106.701668, 10.764901, '2025‑03‑04 11:45:00', '2025‑03‑04 11:45:00'),
(1, 3.00, 28.00, 280.00, N'202 Điện Biên Phủ, Phường 15, Quận Bình Thạnh, TP.HCM', 106.710449, 10.801057, '2025‑03‑05 14:00:00', '2025‑03‑05 14:00:00');
GO
INSERT INTO [dbo].[Area] (ParkingLotId, AreaName, TotalFloor, Status, RentalType) VALUES
(1, N'Khu vực A', 3, 1, 1),
(1, N'Khu vực B', 1, 1, 2),
(1, N'Khu vực C', 3, 1, 1),
(2, N'Khu vực A', 2, 1, 2),
(2, N'Khu vực B', 5, 1, 1),
(3, N'Khu vực A', 3, 1, 2),
(3, N'Khu vực B', 1, 1, 1),
(3, N'Khu vực C', 1, 1, 1),
(4, N'Khu vực A', 2, 1, 1),
(4, N'Khu vực B', 3, 1, 2),
(5, N'Khu vực A', 4, 1, 1),
(5, N'Khu vực B', 4, 1, 1);
GO
INSERT INTO dbo.Floor (AreaId, FloorName, NumberEmptyParkingSpace, NumberUsedParkingSpace, TotalParkingSpace, Status) VALUES
(1, N'Tầng 1', 0, 0, 0, 1),
(1, N'Tầng 2', 0, 0, 0, 1),
(1, N'Tầng 3', 0, 0, 0, 1),
(2, N'Tầng 1', 0, 0, 0, 1),
(2, N'Tầng 2', 0, 0, 0, 1),
(2, N'Tầng 3', 0, 0, 0, 1),
(3, N'Tầng 1', 0, 0, 0, 1),
(3, N'Tầng 2', 0, 0, 0, 1),
(3, N'Tầng 3', 0, 0, 0, 1),
(4, N'Tầng 1', 0, 0, 0, 1),
(4, N'Tầng 2', 0, 0, 0, 1),
(4, N'Tầng 3', 0, 0, 0, 1),
(5, N'Tầng 1', 0, 0, 0, 1),
(5, N'Tầng 2', 0, 0, 0, 1),
(5, N'Tầng 3', 0, 0, 0, 1);
GO
INSERT INTO dbo.ParkingSpace (FloorId, ParkingSpaceName, Status) VALUES
-- Floor 1
(1, N'P01-01', 1),(1, N'P01-02', 1),(1, N'P01-03', 1),(1, N'P01-04', 1),(1, N'P01-05', 1),
(1, N'P01-06', 1),(1, N'P01-07', 2),(1, N'P01-08', 2),(1, N'P01-09', 2),(1, N'P01-10', 2),
-- Floor 2
(2, N'P02-01', 1),(2, N'P02-02', 1),(2, N'P02-03', 1),(2, N'P02-04', 1),(2, N'P02-05', 1),
(2, N'P02-06', 1),(2, N'P02-07', 2),(2, N'P02-08', 2),(2, N'P02-09', 2),(2, N'P02-10', 2),
-- Floor 3
(3, N'P03-01', 1),(3, N'P03-02', 1),(3, N'P03-03', 1),(3, N'P03-04', 1),(3, N'P03-05', 1),
(3, N'P03-06', 1),(3, N'P03-07', 2),(3, N'P03-08', 0),(3, N'P03-09', 2),(3, N'P03-10', 2),
-- Floor 4
(4, N'P04-01', 1),(4, N'P04-02', 1),(4, N'P04-03', 1),(4, N'P04-04', 1),(4, N'P04-05', 1),
(4, N'P04-06', 1),(4, N'P04-07', 2),(4, N'P04-08', 2),(4, N'P04-09', 2),(4, N'P04-10', 2)
GO
UPDATE ParkingSpace
SET Status = 0
GO
UPDATE F
SET NumberEmptyParkingSpace = ISNULL(P.TotalSpaces, 0), NumberUsedParkingSpace = 0, TotalParkingSpace = ISNULL(P.TotalSpaces,0)
FROM dbo.Floor AS F
LEFT JOIN (
    SELECT FloorId, COUNT(*) AS TotalSpaces
    FROM dbo.ParkingSpace
    GROUP BY FloorId
) AS P
    ON F.FloorId = P.FloorId;
GO
-- Contract 1: Active, 3 tháng (2025-03-01 đến 2025-06-01)
INSERT INTO dbo.Contract 
    (CarId, ParkingSpaceId, StartDate, EndDate, Status, CreatedDate, UpdatedDate, Note)
VALUES 
    (1, 1, '2025-03-01', '2025-06-01', 1, '2025-03-01 08:00:00', '2025-03-01 08:00:00', N'Hợp đồng đang hoạt động, thời hạn 3 tháng');

-- Hợp đồng 2: Đã hết hạn, 3 tháng (2025-01-15 đến 2025-04-15)
INSERT INTO dbo.Contract 
    (CarId, ParkingSpaceId, StartDate, EndDate, Status, CreatedDate, UpdatedDate, Note)
VALUES 
    (2, 2, '2025-01-15', '2025-04-15', 3, '2025-01-15 08:00:00', '2025-04-15 08:00:00', N'Hợp đồng đã hết hạn, thời hạn 3 tháng');

-- Hợp đồng 3: Không hoạt động (Đang chờ thanh toán), 3 tháng (2025-02-01 đến 2025-05-01)
INSERT INTO dbo.Contract 
    (CarId, ParkingSpaceId, StartDate, EndDate, Status, CreatedDate, UpdatedDate, Note)
VALUES 
    (3, 3, '2025-02-01', '2025-05-01', 2, '2025-02-01 08:00:00', '2025-02-01 08:00:00', N'Hợp đồng không hoạt động (đang chờ), thời hạn 3 tháng');

-- Hợp đồng 4: Đang hoạt động, 3 tháng (2025-04-01 đến 2025-07-01)
INSERT INTO dbo.Contract 
    (CarId, ParkingSpaceId, StartDate, EndDate, Status, CreatedDate, UpdatedDate, Note)
VALUES 
    (4, 4, '2025-04-01', '2025-07-01', 1, '2025-04-01 08:00:00', '2025-04-01 08:00:00', N'Hợp đồng đang hoạt động, thời hạn 3 tháng');

-- Hợp đồng 5: Không hoạt động (Đang chờ thanh toán), 3 tháng (2025-05-01 đến 2025-08-01)
INSERT INTO dbo.Contract 
    (CarId, ParkingSpaceId, StartDate, EndDate, Status, CreatedDate, UpdatedDate, Note)
VALUES 
    (5, 5, '2025-05-01', '2025-08-01', 2, '2025-05-01 08:00:00', '2025-05-01 08:00:00', N'Hợp đồng không hoạt động (đang chờ), thời hạn 3 tháng');
GO
-- Thanh toán cho Hợp đồng 1 (Đang hoạt động: Đã thanh toán)
INSERT INTO dbo.PaymentContract 
    (ContractId, StartDate, EndDate, PricePerMonth, PaymentMethod, PaymentDate, PaymentAmount, Status, Note, CreatedDate, UpdatedDate)
VALUES 
    (1, '2025-03-01', '2025-06-01', 300.00, N'Thẻ tín dụng', '2025-03-01 09:00:00', 900.00, 1, N'Đã thanh toán đầy đủ', '2025-03-01 09:00:00', '2025-03-01 09:00:00');

-- Thanh toán cho Hợp đồng 2 (Đã hết hạn: Đã thanh toán)
INSERT INTO dbo.PaymentContract 
    (ContractId, StartDate, EndDate, PricePerMonth, PaymentMethod, PaymentDate, PaymentAmount, Status, Note, CreatedDate, UpdatedDate)
VALUES 
    (2, '2025-01-15', '2025-04-15', 350.00, N'Tiền mặt', '2025-01-15 09:00:00', 1050.00, 1, N'Đã thanh toán đầy đủ', '2025-01-15 09:00:00', '2025-01-15 09:00:00');

-- Thanh toán cho Hợp đồng 3 (Không hoạt động: Đang chờ thanh toán)
INSERT INTO dbo.PaymentContract 
    (ContractId, StartDate, EndDate, PricePerMonth, PaymentMethod, PaymentDate, PaymentAmount, Status, Note, CreatedDate, UpdatedDate)
VALUES 
    (3, '2025-02-01', '2025-05-01', 280.00, N'Chuyển khoản ngân hàng', NULL, 840.00, 2, N'Đang chờ thanh toán', '2025-02-01 10:00:00', '2025-02-01 10:00:00');

-- Thanh toán cho Hợp đồng 4 (Đang hoạt động: Đã thanh toán)
INSERT INTO dbo.PaymentContract 
    (ContractId, StartDate, EndDate, PricePerMonth, PaymentMethod, PaymentDate, PaymentAmount, Status, Note, CreatedDate, UpdatedDate)
VALUES 
    (4, '2025-04-01', '2025-07-01', 400.00, N'Thẻ tín dụng', '2025-04-01 10:00:00', 1200.00, 1, N'Đã thanh toán đầy đủ', '2025-04-01 10:00:00', '2025-04-01 10:00:00');

-- Thanh toán cho Hợp đồng 5 (Không hoạt động: Đang chờ thanh toán)
INSERT INTO dbo.PaymentContract 
    (ContractId, StartDate, EndDate, PricePerMonth, PaymentMethod, PaymentDate, PaymentAmount, Status, Note, CreatedDate, UpdatedDate)
VALUES 
    (5, '2025-05-01', '2025-08-01', 320.00, N'Chuyển khoản ngân hàng', NULL, 960.00, 2, N'Đang chờ thanh toán', '2025-05-01 10:00:00', '2025-05-01 10:00:00');
GO