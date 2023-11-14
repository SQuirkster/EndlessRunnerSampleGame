#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

/** List of errors that can be returned from the SDK. */
typedef NS_ENUM(NSInteger, RCARecaptchaErrorCode) {
  /** Unknown error occurred during the workflow. */
  RCARecaptchaErrorCodeUnknown = 0,
  /** reCAPTCHA encountered an internal issue. */
  RCARecaptchaErrorCodeInternalError = 1,
  /** There was a network error during one of the operations. */
  RCARecaptchaErrorCodeNetworkError = 2,
  /** There was a network error during one of the operations. */
  RCARecaptchaErrorCodeTwoFactorInvalidRequestToken = 3,
  /** There was a network error during one of the operations. */
  RCARecaptchaErrorCodeTwoFactorInvalidPin = 4,
  /** There was a network error during one of the operations. */
  RCARecaptchaErrorCodeTwoFactorChallengeExpired = 5,
  /** There was a network error during one of the operations. */
  RCARecaptchaErrorCodeTwoFactorPinMismatch = 6,
  /** There was a network error during one of the operations. */
  RCARecaptchaErrorCodeTwoFactorSessionExpired = 7,
  /** There was a network error during one of the operations. */
  RCARecaptchaErrorCodeTwoFactorAttemptsExhausted = 8,
  /** The user cancelled the verification. */
  RCARecaptchaErrorCodeTwoFactorVerificationCancelled = 9,

  /**
   * Deprecated codes: 10, 11, 12.
   */

  /**
   *  2FA Workflow was aborted due to a problem. You can retrieve a reCAPTCHA
   *  token with more information on the `userInfo` property using
   *  `workflowAbortedTokenKey`.
   */
  RCARecaptchaErrorCodeTwoFactorAborted = 13,
};

/** Error class for reCAPTCHA Events. */
@interface RCARecaptchaError : NSError

@property(nonatomic) NSString *errorMessage;

/**
 * If the returned error code is of type `Aborted` you can use this key to
 * get the aborted token out of the `userInfo` property. The token is stored
 * inside an instance of `RecaptchaToken`.
 */
@property(class, nonatomic, readonly) NSString *workflowAbortedTokenKey;

@property(nonatomic) long long networkErrorCode;

/**
 * Builds a new reCAPTCHA Error object.
 */
- (instancetype)initWithDomain:(NSErrorDomain)domain
                          code:(NSInteger)code
                      userInfo:(NSDictionary<NSErrorUserInfoKey, id> *_Nullable)dict
                  errorMessage:(NSString *)message;

- (instancetype)init NS_UNAVAILABLE;

/** For debug purposes, prints a human friendly description of the error. */
- (NSString *)description;

@end

NS_ASSUME_NONNULL_END
