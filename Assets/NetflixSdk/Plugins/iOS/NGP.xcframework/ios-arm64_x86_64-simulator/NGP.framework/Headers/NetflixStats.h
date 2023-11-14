//
//  NetflixStats.h
//  NGP
//
//  Created by Rob Harris on 11/14/22.
//

#import <Foundation/Foundation.h>

@class NetflixStatItem;
@class NetflixSubmitStatResult;
@class NetflixAggregatedStatResult;

typedef void (^NetflixSubmitStatCallback)(NetflixSubmitStatResult * _Nonnull response);
typedef void(^NetflixAggregatedStatCallback)(NetflixAggregatedStatResult* _Nonnull result);

NS_ASSUME_NONNULL_BEGIN

@interface NetflixStats : NSObject

+ (void)submitStat:(NetflixStatItem *)statItem;

+ (void)submitStatNow:(NetflixStatItem *)statItem callback:(NetflixSubmitStatCallback)callback;

+ (void)aggregatedStatWithName:(NSString *)statName callback:(NetflixAggregatedStatCallback)callback;

@end

NS_ASSUME_NONNULL_END
