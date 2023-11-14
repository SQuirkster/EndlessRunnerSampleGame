//
//  NetflixLeaderboard.h
//  NGP
//
//  Created by Rob Harris on 11/14/22.
//

#import <Foundation/Foundation.h>
#import <NGP/NGPApiConstants.h>
#import <NGP/NGPApiTypes.h>

NS_ASSUME_NONNULL_BEGIN

@interface NetflixLeaderboard : NSObject

+ (void)topEntriesWithLeaderboardName:(NSString *)leaderboardName maxEntries:(NSInteger)maxEntries
                             callback:(NetflixLeaderboardEntriesCallback)callback;

+ (void)moreEntriesWithLeaderboardName:(NSString *)leaderboardName maxEntries:(NSInteger)maxEntries
                                cursor:(NSString *)cursor direction:(NetflixFetchDirection) direction callback:(NetflixLeaderboardEntriesCallback)callback;

+ (void)entriesAroundCurrentPlayerWithLeaderboardName:(NSString *)leaderboardName maxEntries:(NSInteger)maxEntries callback:(NetflixLeaderboardEntriesCallback)callback;

+ (void)currentPlayerEntryWithLeaderboardName:(NSString *)leaderboardName callback:(NetflixLeaderboardEntryCallback)callback;

+ (void)leaderboardInfoWithLeaderboardName:(NSString *)leaderboardName callback:(NetflixLeaderboardInfoCallback)callback;

@end

NS_ASSUME_NONNULL_END
