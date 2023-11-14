//
//  NetflixSDKEventHandler.h
//  NGP
//
//  Created by Rob Harris on 8/25/21.
//

#import <Foundation/Foundation.h>

@class NetflixSDKState;

NS_ASSUME_NONNULL_BEGIN

@protocol NetflixSDKEventHandler <NSObject>
@required
- (void)onUserStateChange:(NetflixSDKState *)sdkState;

- (void)onNetflixUIVisible;

- (void)onNetflixUIHidden;

@end

NS_ASSUME_NONNULL_END
