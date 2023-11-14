//
//  NetflixMessaging.h
//  NGP
//
//  Created by Allan Zhou on 9/26/21.
//  Copyright (c) 2021 Netflix, Inc.  All rights reserved.
//

#import <Foundation/Foundation.h>
#import <NGP/NGPApiConstants.h>

NS_ASSUME_NONNULL_BEGIN

/* previsional, not ready yet */
@interface NetflixMessaging : NSObject

+ (void)onPushToken:(NSString *)tokenString;

+ (void)OnMessagingEvent:(MessagingEventType)eventType jsonString:(NSString *)jsonString NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixMessaging OnMessagingEvent] API is deprecated. Please use the +[NetflixMessaging onMessagingEvent] API instead.");
+ (void)onMessagingEvent:(MessagingEventType)eventType jsonString:(NSString *)jsonString;

+ (void)onDeeplinkReceived:(NSString *)deepLinkURL processedByGame:(BOOL)processedByGame;

@end

NS_ASSUME_NONNULL_END
