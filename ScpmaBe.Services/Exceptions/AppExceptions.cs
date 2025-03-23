namespace ScpmBe.Services.Exceptions
{
    public static class AppExceptions
    {
        public static AppException BadRequestUserExists()
        {
            return new BadRequestException("MSG01", "Username already exists");
        }
        public static AppException BadRequestUsernameIsInvalid()
        {
            return new BadRequestException("MSG02", "Username is invalid");
        }
        public static AppException NotFoundAccount()
        {
            return new NotFoundException("MSG03", "Account not found.");
        }

        public static AppException NotFoundId()
        {
            return new NotFoundException("MSG04", "Id not found.");
        }
        public static AppException IdExists()
        {
            return new NotFoundException("MSG05", "Id already exists.");
        }

        public static AppException NotFoundOnwerId()
        {
            return new NotFoundException("MSG06", "OwnerId not found.");
        }

        public static AppException NotFoundAccountId()
        {
            return new NotFoundException("MSG07", "AccountId not found.");
        }

        public static AppException NotFoundFeedBackId()
        {
            return new NotFoundException("MSG08", "FeedbackId not found.");
        }

        public static AppException NotFoundCustomerId()
        {
            return new NotFoundException("MSG09", "CustomerId not found.");
        }

        public static AppException NotFoundCar()
        {
            return new NotFoundException("MSG10", "CarId not found.");
        }

        public static AppException BadRequestLicensePlateExists()
        {
            return new BadRequestException("MSG11", "LicensePlate already exists");
        }

        public static AppException NotFoundContract()
        {
            return new NotFoundException("MSG12", "ContractId not found.");
        }

        public static AppException NotFoundArea()
        {
            return new NotFoundException("MSG13", "AreaId not found.");
        }

        public static AppException NotFoundParkingLot()
        {
            return new NotFoundException("MSG14", "ParkingLotId not found.");
        }

        public static AppException NotFoundParkingSpace()
        {
            return new NotFoundException("MSG15", "ParkingSpaceId not found.");
        }

        public static AppException NotFoundFloor()
        {
            return new NotFoundException("MSG16", "FloorId not found.");
        }
        public static AppException NotFoundEntryExitLog()
        {
            return new NotFoundException("MSG17", "EntryExitLogId not found.");
        }

        public static AppException NotFoundPhoneNumber()
        {
            return new NotFoundException("MSG18", "Phone number cannot be null or empty.");
        }

        public static AppException WrongNumber()
        {
            return new BadRequestException("MSG19", "Wrong number.");
        }

        public static AppException EmptySearchFields()
        {
            return new BadRequestException("MSG20", "Please fill in information.");
        }
    }
}
