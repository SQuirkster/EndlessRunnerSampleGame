#import <Foundation/Foundation.h>
#import "RecaptchaActionType.h"

/**
 * Action intended to be protected by reCAPTCHA. An instance of this object
 * should be passed to RecaptchaClient.execute.
 */
@interface RecaptchaAction : NSObject

/** Action that is intended for reCAPTCHA to protect. */
@property(nonatomic, readonly) RecaptchaActionType action;

/** A user customized action. */
@property(nonatomic, readonly) NSString *customAction;

/** Extra parameters to send during call to execute. */
@property(nonatomic, readonly) NSDictionary *extraParameters;

/** Creates an instance with a predefined reCAPTCHA action. */
- (instancetype)initWithAction:(RecaptchaActionType)action;

/** Creates an instance with a custom action in the form of a String. */
- (instancetype)initWithCustomAction:(NSString *)customAction;

/** Creates an instance with a predefined reCAPTCHA action and extra parameters. */
- (instancetype)initWithAction:(RecaptchaActionType)action
               extraParameters:(NSDictionary *)extraParameters;

/** Creates an instance with a custom action in the form of a String and extra parameters. */
- (instancetype)initWithCustomAction:(NSString *)customAction
                     extraParameters:(NSDictionary *)extraParameters;

- (instancetype)init NS_UNAVAILABLE;

@end
