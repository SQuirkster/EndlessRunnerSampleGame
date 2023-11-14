//
//  NetflixSDKState.h
//  NGP
//
//  Created by Rob Harris on 7/30/21.
//  Copyright (c) 2021 Netflix, Inc.  All rights reserved.
//

#import <Foundation/Foundation.h>

@class NetflixProfile;

NS_ASSUME_NONNULL_BEGIN

@interface NetflixSDKState : NSObject

@property (nonatomic, nullable, readonly) NetflixProfile *currentProfile;
@property (nonatomic, nullable, readonly) NetflixProfile *previousProfile;

+ (instancetype)new NS_UNAVAILABLE;
- (instancetype)init NS_UNAVAILABLE;

@end

NS_ASSUME_NONNULL_END
