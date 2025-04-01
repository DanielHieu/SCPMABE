

namespace ScpmBe.Services.Exceptions
{
    public static class AppExceptions
    {
        public static AppException BadRequest(string message)
        {
            return new BadRequestException("MSG00", message);
        }

        public static AppException BadRequestUserExists()
        {
            return new BadRequestException("MSG01", "Tên người dùng hoặc Email đã tồn tại");
        }
        public static AppException BadRequestUsernameIsInvalid()
        {
            return new BadRequestException("MSG02", "Tên người dùng không hợp lệ");
        }
        public static AppException NotFoundAccount()
        {
            return new NotFoundException("MSG03", "Không tìm thấy tài khoản.");
        }

        public static AppException NotFoundId()
        {
            return new NotFoundException("MSG04", "Không tìm thấy Id.");
        }
        public static AppException IdExists()
        {
            return new NotFoundException("MSG05", "Id đã tồn tại.");
        }

        public static AppException NotFoundOnwerId()
        {
            return new NotFoundException("MSG06", "Không tìm thấy OwnerId.");
        }

        public static AppException NotFoundAccountId()
        {
            return new NotFoundException("MSG07", "Không tìm thấy AccountId.");
        }

        public static AppException NotFoundFeedBackId()
        {
            return new NotFoundException("MSG08", "Không tìm thấy FeedbackId.");
        }

        public static AppException NotFoundCustomerId()
        {
            return new NotFoundException("MSG09", "Không tìm thấy CustomerId.");
        }

        public static AppException NotFoundCar()
        {
            return new NotFoundException("MSG10", "Không tìm thấy CarId.");
        }

        public static AppException BadRequestLicensePlateExists()
        {
            return new BadRequestException("MSG11", "Biển số xe đã tồn tại");
        }

        public static AppException NotFoundContract()
        {
            return new NotFoundException("MSG12", "Không tìm thấy ContractId.");
        }

        public static AppException NotFoundArea()
        {
            return new NotFoundException("MSG13", "Không tìm thấy AreaId.");
        }

        public static AppException NotFoundParkingLot()
        {
            return new NotFoundException("MSG14", "Không tìm thấy ParkingLotId.");
        }

        public static AppException NotFoundParkingSpace()
        {
            return new NotFoundException("MSG15", "Không tìm thấy ParkingSpaceId.");
        }

        public static AppException NotFoundFloor()
        {
            return new NotFoundException("MSG16", "Không tìm thấy FloorId.");
        }
        public static AppException NotFoundEntryExitLog()
        {
            return new NotFoundException("MSG17", "Không tìm thấy EntryExitLogId.");
        }

        public static AppException NotFoundPhoneNumber()
        {
            return new NotFoundException("MSG18", "Số điện thoại không được để trống.");
        }

        public static AppException WrongNumber()
        {
            return new BadRequestException("MSG19", "Sai số.");
        }

        public static AppException EmptySearchFields()
        {
            return new BadRequestException("MSG20", "Vui lòng điền thông tin.");
        }

        public static AppException PaymentAlreadyCompleted()
        {
            return new BadRequestException("MSG21", "Hợp đồng thanh toán đã hoàn thành.");
        }

        public static AppException AreaNameExisted()
        {
            return new BadRequestException("MSG22", "Tên khu vực đã tồn tại.");
        }

        public static AppException AccountNotActivated()
        {
            return new BadRequestException("MSG23", "Tài khoản chưa được kích hoạt.");
        }

        public static AppException FeedBackAlreadyReponsed()
        {
            return new BadRequestException("MSG24", "Phản hồi đã được trả lời.");
        }

        public static AppException FloorNameExisted()
        {
            return new BadRequestException("MSG25", "Tên tầng đã tồn tại.");
        }

        public static AppException ParkingSpaceNameExisted()
        {
            return new BadRequestException("MSG26", "Tên chỗ đậu xe đã tồn tại.");
        }

        internal static Exception ParkingLotNameExisted()
        {
            return new BadRequestException("MSG27", "Tên bãi đậu xe đã tồn tại.");
        }
    }
}
