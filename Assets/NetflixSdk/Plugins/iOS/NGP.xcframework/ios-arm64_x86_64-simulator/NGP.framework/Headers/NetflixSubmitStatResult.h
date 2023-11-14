//
//  NetflixSubmitStatResult.h
//  NGP
//
//  Created by Rob Harris on 8/22/22.
//

#import <Foundation/Foundation.h>
#import <NGP/NGPApiConstants.h>

@class NetflixStatItem;

NS_ASSUME_NONNULL_BEGIN

@interface NetflixSubmitStatResult : NSObject
@property (nonatomic, readonly) NetflixStatsStatus status;
@property (nullable, nonatomic, readonly) NetflixStatItem *submittedStat;
@property (nullable, nonatomic, readonly) NetflixStatItem *aggregatedStat;

- (NSString *)bridgeRepresentation;
@end

NS_ASSUME_NONNULL_END
