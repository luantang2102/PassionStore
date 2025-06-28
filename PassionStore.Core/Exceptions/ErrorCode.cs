using Microsoft.AspNetCore.Http;

namespace PassionStore.Core.Exceptions
{
    public class ErrorCode
    {
        /// <summary>
        /// Custom error code, message, and status.
        /// </summary>
        // User related errors (100-199)
        // Database integration errors (500 - 599)
        public static readonly ErrorCode SAVE_ERROR = new(500, "Error saving to database", StatusCodes.Status500InternalServerError);

        // User related errors (600 - 699)
        public static readonly ErrorCode ACCOUNT_NOT_FOUND = new(600, "Account not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode USER_NOT_FOUND = new(601, "User not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode INVALID_OLD_PASSWORD = new(602, "Invalid old password", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode IDENTITY_CREATION_FAILED = new(603, "Create user failed", StatusCodes.Status500InternalServerError);
        public static readonly ErrorCode SELF_ACCESS_DENIED = new(604, "Cannot access or modify your own user details in this administrative context", StatusCodes.Status403Forbidden);
        public static readonly ErrorCode USER_ALREADY_EXISTS = new(605, "User already exists", StatusCodes.Status409Conflict);

        // Access and token related errors (700 - 799)
        public static readonly ErrorCode UNAUTHORIZED_ACCESS = new(700, "Unauthorized access", StatusCodes.Status401Unauthorized);
        public static readonly ErrorCode ACCESS_DENIED = new(701, "Access denied to view or modify this resource", StatusCodes.Status403Forbidden);
        public static readonly ErrorCode INVALID_CLAIM = new(702, "Invalid claim", StatusCodes.Status404NotFound);
        public static readonly ErrorCode INVALID_CREDENTIALS = new(704, "Invalid credentials", StatusCodes.Status401Unauthorized);
        public static readonly ErrorCode INVALID_REFRESH_TOKEN = new(705, "Invalid refresh token", StatusCodes.Status401Unauthorized);
        public static readonly ErrorCode REFRESH_TOKEN_EXPIRED = new(706, "Refresh token expired", StatusCodes.Status401Unauthorized);
        public static readonly ErrorCode REFRESH_TOKEN_NOT_FOUND = new(707, "Refresh token not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode PASSWORDS_DO_NOT_MATCH = new(708, "Passwords do not match", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode EMAIL_NOT_VERIFIED = new(709, "Email not verified", StatusCodes.Status422UnprocessableEntity);
        public static readonly ErrorCode EMAIL_ALREADY_VERIFIED = new(710, "Email already verified", StatusCodes.Status409Conflict);
        public static readonly ErrorCode INVALID_VERIFICATION_CODE = new(711, "Invalid verification code", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode VERIFICATION_CODE_ALREADY_USED = new(712, "Verification code already used", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode VERIFICATION_CODE_EXPIRED = new(713, "Verification code expired", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode TOO_MANY_REQUESTS = new(714, "Too many requests", StatusCodes.Status429TooManyRequests);
        public static readonly ErrorCode GOOGLE_LOGIN_FAILED = new(715, "Google login failed", StatusCodes.Status400BadRequest);

        // Validation related errors (800 - 899)
        public static readonly ErrorCode VALIDATION_ERROR = new(800, "Validation error", StatusCodes.Status422UnprocessableEntity);

        // Product related errors (600-699)
        public static readonly ErrorCode PRODUCT_NOT_FOUND = new(600, "Product not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode DUPLICATE_PRODUCT = new(601, "Product already existed", StatusCodes.Status409Conflict);
        public static readonly ErrorCode PRODUCT_IN_CART = new(602, "Cannot delete product because it exists in one or more carts", StatusCodes.Status400BadRequest);

        // Cloudinary related errors (700-799)
        public static readonly ErrorCode CLOUDINARY_UPLOAD_FAILED = new(700, "Cloudinary upload failed", StatusCodes.Status500InternalServerError);
        public static readonly ErrorCode CLOUDINARY_DELETE_FAILED = new(701, "Cloudinary delete failed", StatusCodes.Status500InternalServerError);

        // Category related errors (800-899)
        public static readonly ErrorCode CATEGORY_NOT_FOUND = new(800, "Category not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode DUPLICATE_CATEGORY = new(801, "Category already existed", StatusCodes.Status409Conflict);
        public static readonly ErrorCode PARENT_CATEGORY_NOT_FOUND = new(802, "Parent category not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode CATEGORY_CIRCULAR_REFERENCE = new(803, "Category circular reference", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode CATEGORY_HAS_SUBCATEGORIES = new(804, "Category has subcategories, cannot delete", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode CATEGORY_HAS_PRODUCTS = new(804, "Category has products, cannot delete", StatusCodes.Status400BadRequest);

        // Rating related errors (900-999)
        public static readonly ErrorCode RATING_NOT_FOUND = new(900, "Rating not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode RATING_ALREADY_EXISTS = new(901, "Rating already exists", StatusCodes.Status409Conflict);

        // Cart related errors (1000-1099)
        public static readonly ErrorCode CART_NOT_FOUND = new(1000, "Cart item not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode CART_ITEM_NOT_FOUND = new(1001, "Cart item not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode INSUFFICIENT_STOCK = new(1002, "Insufficient stock", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode CART_EMPTY = new(1003, "Cart empty", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode CART_CREATION_FAILED = new(1004, "Failed to create cart", StatusCodes.Status400BadRequest);

        // Order related errors (1100-1199) 
        public static readonly ErrorCode ORDER_NOT_FOUND = new(1100, "Order not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode ORDER_NOT_CANCELLABLE = new(1101, "Order cannot be cancelled", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode INVALID_STATUS_TRANSITION = new(1102, "Invalid status transition", StatusCodes.Status400BadRequest);

        // Payment related errors (1200-1299)
        public static readonly ErrorCode PAYMENT_CREATION_FAILED = new(1200, "Payment creation failed", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode PAYMENT_CANCELLATION_FAILED = new(1201, "Payment cancellation failed", StatusCodes.Status400BadRequest);

        // Product variant related errors (1300-1399)
        public static readonly ErrorCode PRODUCT_VARIANT_NOT_FOUND = new(1300, "Product variant not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode PRODUCT_VARIANT_ALREADY_EXISTS = new(1301, "Product variant already exists", StatusCodes.Status409Conflict);
        public static readonly ErrorCode PRODUCT_VARIANT_IN_CART = new(1302, "Cannot delete product variant because it exists in one or more carts", StatusCodes.Status400BadRequest);
        public static readonly ErrorCode CANNOT_DELETE_DEFAULT_VARIANT = new(1303, "Cannot delete default product variant", StatusCodes.Status400BadRequest);

        // Color related errors (1400-1499)
        public static readonly ErrorCode COLOR_NOT_FOUND = new(1400, "Color not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode DUPLICATE_COLOR = new(1401, "Color already exists", StatusCodes.Status409Conflict);
        public static readonly ErrorCode COLOR_IN_USE = new(1402, "Color is in use by one or more product variants", StatusCodes.Status400BadRequest);

        // Size related errors (1500-1599)
        public static readonly ErrorCode SIZE_NOT_FOUND = new(1500, "Size not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode SIZE_IN_USE = new(1501, "Size is in use by one or more product variants", StatusCodes.Status400BadRequest);

        // Brand related errors (1600-1699)
        public static readonly ErrorCode BRAND_NOT_FOUND = new(1600, "Brand not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode BRAND_IN_USE = new(1601, "Brand is in use by one or more products", StatusCodes.Status400BadRequest);

        // User profile and address related errors (1700-1799)
        public static readonly ErrorCode USER_PROFILE_NOT_FOUND = new(1700, "User profile not found", StatusCodes.Status404NotFound);
        public static readonly ErrorCode ADDRESS_NOT_FOUND = new(1701, "Address not found", StatusCodes.Status404NotFound);

        // Images related errors (1800-1899)
        public static readonly ErrorCode INVALID_MAIN_IMAGE_COUNT = new(1800, "Invalid main image count", StatusCodes.Status400BadRequest);

        /// <summary>
        /// Atributes for error code, message, and status. 
        /// </summary>
        private readonly int _code;
        private readonly string _message;
        private readonly int _status;

        public ErrorCode(int code, string message, int status)
        {
            _code = code;
            _message = message;
            _status = status;
        }

        public int GetCode() => _code;
        public string GetMessage() => _message;
        public int GetStatus() => _status;
    }
}
