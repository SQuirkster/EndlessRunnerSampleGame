#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#if __has_include(<FBLPromises/FBLPromises.h>)
#import <FBLPromises/FBLPromises.h>
#elif __has_include(<FBLPromises.h>)
#import <FBLPromises.h>
#endif

@class RecaptchaAction;
@class RCARecaptchaError;
@class RecaptchaToken;
@class RecaptchaVerificationHandler;

NS_ASSUME_NONNULL_BEGIN

/** Common reCAPTCHA callback:
 * @param recaptchaToken representing a reCAPTCHA token as a RecaptchaToken
 * object. May be nil if an error occurred.
 * @param recaptchaError A RCARecaptchaError object that may have arised from a
 * reCAPTCHA operation.
 */
typedef void (^RecaptchaCallback)(RecaptchaToken *_Nullable recaptchaToken,
                                  RCARecaptchaError *_Nullable recaptchaError);

/** Interface to interact with reCAPTCHA. */
@interface RecaptchaClient : NSObject

/**
 * Builds a new reCAPTCHA Client for the given SiteKey.
 */
- (instancetype)initWithSiteKey:(NSString *)siteKey NS_DESIGNATED_INITIALIZER;

- (instancetype)init NS_UNAVAILABLE;

/**
 * All The methods on this category are only available when integrating with the
 * light client and linking manually against Promises.
 */
#if __has_include(<FBLPromises/FBLPromises.h>) || __has_include(<FBLPromises.h>)
/**
 * Collects the data from the signals running.
 * @return A Promise to a string that represents a reCAPTCHA token.
 */
- (FBLPromise<RecaptchaToken *> *)execute:(RecaptchaAction *)action;

/**
 * If you prefer to show your own UI, you can invoke this method that will
 * return a reCAPTCHA Verification Handle, whenever you have the user's provided
 * input you can invoke @c VerificationHandle#verifyPin to complete the 2FA
 * verification process.
 * @param requestToken 2FA Request Token received during Create Assessment phase.
 * @return A promise to a reCAPTCHA Verification Handle.
 */
- (FBLPromise<RecaptchaVerificationHandler *> *)challengeAccountWithRequestToken:
    (NSString *)requestToken;

#endif

/**
 * Collects the data from the signals running.
 * @param onFinishedExecute Callback that will be invoked with the resulting
 * reCAPTCHA token or any encoutnered error.
 */
- (void)execute:(RecaptchaAction *)action onFinishedExecute:(RecaptchaCallback)onFinishedExecute;

/**
 * If you prefer to show your own UI, you can invoke this method that will return
 * a reCAPTCHA Verification Handle, whenever you have the user's provided
 * input you can invoke @c VerificationHandle#verifyPin to complete the 2FA
 * verification process.
 * @param requestToken 2FA Request Token received during Create Assessment phase.
 * @param onChallengeDone callback that will be invoked with the resulting
 * RecaptchaVerificationHandler or error.
 */
- (void)challengeAccountWithRequestToken:(NSString *)requestToken
                         onChallengeDone:(void (^)(RecaptchaVerificationHandler *_Nullable,
                                                   RCARecaptchaError *_Nullable))onChallengeDone;

@end

NS_ASSUME_NONNULL_END
