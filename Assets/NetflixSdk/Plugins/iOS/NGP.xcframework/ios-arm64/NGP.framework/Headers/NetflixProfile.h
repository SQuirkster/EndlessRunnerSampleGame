//
//  NetflixProfile.h
//  NGP
//
//  Created by Rob Harris on 7/16/21.
//  Copyright (c) 2021 Netflix, Inc.  All rights reserved.
//

#import <Foundation/Foundation.h>
#import <NGP/NGPApiConstants.h>

NS_ASSUME_NONNULL_BEGIN

@class Locale;
/**
 * <p>Represents a Netflix user.</p>
 *
 */
@interface NetflixProfile : NSObject
/**
 * The unique player ID. This ID is not intended for display purposes but for using as a primary
 * key for any player data lookup or storage, and as the player identifier for logging use
 * cases. This field returns the same value as the playerId field in the object returned by the
 * NetflixPlayerIdentity.current API.
 */
@property (nonatomic, copy, readonly) NSString * playerId;

@property (nonatomic, copy, readonly) NSString *gamerProfileId NGDP_API_DEPRECATED_WITH_MESSAGE("The gamerProfileId is a unique identifier that game apps can use to associate state and information with players in their game. The gamerProfileId will be consistent for a particular Netflix user and profile for multiple game sessions on a single device, as well as across devices.  If an existing game is already using gamerProfileId as a primary key to local or cloud storage, developers can continue using this field. Although this property is deprecated, Netflix is committed to supporting this property for the lifetime of the game where it is already in use.  If a game is currently not using the gamerProfileId property, and if the use case arises, please use playerId as your primary key instead of gamerProfileId.");
/**
 * <p>Correlation Id for use with 3rd party services</p>
 * @return Id
 */
@property (nullable, nonatomic, copy, readonly) NSString *loggingId NGDP_API_DEPRECATED_WITH_MESSAGE("The loggingId is a unique identifier associated with the Netflix user and profile that must be included in logs. Assuming that no data, other than logs, should be associated with the loggingId, any references to this field can be safely replaced with playerId.This field will be removed from the Netflix SDK in a future release.");
/**
 * <p>Token for use with 3rd party services</p>
 * @return Token
 */
@property (nullable, nonatomic, copy, readonly) NSString *netflixAccessToken;
/**
 * <p>Set game's preferred Locale</p>
 * @return Locale
 */
@property (nonatomic, nullable, strong) Locale *locale;

+ (instancetype)new NS_UNAVAILABLE;
- (instancetype)init NS_UNAVAILABLE;

@end

NS_ASSUME_NONNULL_END
