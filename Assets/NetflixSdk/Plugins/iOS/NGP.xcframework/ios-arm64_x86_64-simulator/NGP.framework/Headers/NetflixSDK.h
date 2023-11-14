//
//  NetflixSDK.h
//  NGP
//
//  Created by Rob Harris on 8/25/21.
//

#import <Foundation/Foundation.h>
#import <NGP/NGPApiConstants.h>
#import <NGP/NGPApiTypes.h>
#import <NGP/NetflixStats.h>

@protocol NetflixSDKEventHandler;

@class Locale;
@class NetflixStatItem;
@class InGameEvent;


NS_ASSUME_NONNULL_BEGIN

@interface NetflixSDK : NSObject

+ (void)checkUserAuth;

+ (void)showNetflixMenu:(NGPLocation)location NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK showNetflixMenu] API is deprecated. Please use the +[NetflixSDK showNetflixAccessButton] API instead.");

+ (void)hideNetflixMenu NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK hideNetflixMenu] API is deprecated. Please use the +[NetflixSDK hideNetflixAccessButton] API instead.");


/*!
 * @brief Requests for NetflixSDK to @b display the Netflix access button over your game's UI.
 *
 * @discussion
 * Calling this will request that the SDK displays its UI to the user over any game UI underneath.
 * However, if the user is in one of several states (e.g., they're not logged into a Netflix account), we will display the login screen regardless
 * of this call. Same goes for the Netflix menu; if the user is actively in the Netflix menu, then we will not disrupt the player's current experience (either by re-showing or hiding).
 *
 * @note The "Netflix access button" is a small UI button displayed by the Netflix SDK on top of your game's UI in the upper right corner.
 *
 * @seealso +[NetflixSDK hideNetflixAccessButton]
 */
+ (void)showNetflixAccessButton;

/*!
 * @brief Requests for NetflixSDK to @b hide the Netflix access button over your game's UI.
 *
 * @discussion
 * Calling this will request that the SDK @b hide its UI from the user.
 * However, if the user is in one of several states (e.g., they're not logged into a Netflix account), we will display the login screen regardless
 * of this call. Same goes for the Netflix menu; if the user is actively in the Netflix menu, then we will not disrupt the player's current experience (either by re-showing or hiding).
 *
 * @note The "Netflix access button" is a small UI button displayed by the Netflix SDK on top of your game's UI in the upper right corner.
 *
 * @seealso +[NetflixSDK showNetflixAccessButton]
 */
+ (void)hideNetflixAccessButton;

+ (void)registerEventReceiver:(id<NetflixSDKEventHandler>)receiver;

+ (void)setLocale:(Locale *)locale;

+ (NSString *)sdkVersion;

+ (void)leaveBreadcrumb:(NSString *)string;

+ (void)logHandledException:(NSException*)exception;

/* push notifications */

+ (void)onPushToken:(NSString *)tokenString NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK onPushToken:] API is deprecated. Please use the +[NetflixMessaging onPushToken:] API instead.");

+ (void)onMessagingEvent:(MessagingEventType)eventType jsonString:(NSString *)jsonString NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK onMessagingEvent:jsonString:] API is deprecated. Please use the +[NetflixMessaging onMessagingEvent:jsonString:] API instead.");

+ (void)onDeeplinkReceived:(NSString *)deepLinkURL processedByGame:(BOOL)processedByGame NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK onDeeplinkReceived:processedByGame:] API is deprecated. Please use the +[NetflixMessaging onDeeplinkReceived:processedByGame:] API instead.");

+ (void)sendInGameEvent:(NSString *)clTypeName withData:(NSString *)eventDataJson NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK sendInGameEvent:withData:] API is deprecated. Please use the +[NetflixSDK logInGameEvent:] API instead.");

+ (void)logInGameEvent:(InGameEvent *)inGameEvent NS_SWIFT_NAME(logInGameEvent(_:));

/* stats */

+ (void)submitStat:(NetflixStatItem *)statItem NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK submitStat:] API is deprecated. Please use the +[NetflixStats submitStat:] API instead.");

+ (void)submitStatNow:(NetflixStatItem *)statItem callback:(NetflixSubmitStatCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK submitStatNow:callback:] API is deprecated. Please use the +[NetflixStats submitStatNow:callback:] API instead.");

+ (void)aggregatedStatWithName:(NSString *)statName callback:(NetflixAggregatedStatCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK aggregatedStatWithName:callback:] API is deprecated. Please use the +[NetflixStats aggregatedStatWithName:callback:] API instead.");

/* leaderboards */

+ (void)topEntriesWithLeaderboardName:(NSString *)leaderboardName maxEntries:(NSInteger)maxEntries callback:(NetflixLeaderboardEntriesCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK topEntriesWithLeaderboardName:maxEntries:callback:] API is deprecated. Please use the +[NetflixLeaderboard topEntriesWithLeaderboardName:maxEntries:callback:] API instead.");

+ (void)moreEntriesWithLeaderboardName:(NSString *)leaderboardName maxEntries:(NSInteger)maxEntries cursor:(NSString *)cursor direction:(NetflixFetchDirection) direction callback:(NetflixLeaderboardEntriesCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK moreEntriesWithLeaderboardName:maxEntries:cursor:direction:callback:] API is deprecated. Please use the +[NetflixLeaderboard moreEntriesWithLeaderboardName:maxEntries:cursor:direction:callback:] API instead.");;

+ (void)entriesAroundCurrentPlayerWithLeaderboardName:(NSString *)leaderboardName maxEntries:(NSInteger)maxEntries callback:(NetflixLeaderboardEntriesCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK entriesAroundCurrentPlayerWithLeaderboardName:maxEntries:callback:] API is deprecated. Please use the +[NetflixLeaderboard topEntriesWithLeaderboardName:maxEntries:callback:] API instead.");

+ (void)currentPlayerEntryWithLeaderboardName:(NSString *)leaderboardName callback:(NetflixLeaderboardEntryCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK currentPlayerEntryWithLeaderboardName:callback:] API is deprecated. Please use the +[NetflixLeaderboard currentPlayerEntryWithLeaderboardName:callback:] API instead.");

+ (void)leaderboardInfoWithLeaderboardName:(NSString *)leaderboardName callback:(NetflixLeaderboardInfoCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK leaderboardInfoWithLeaderboardName:callback:] API is deprecated. Please use the +[NetflixLeaderboard leaderboardInfoWithLeaderboardName:callback:] API instead.");

/* cloud saves */

+ (void)getSlotIds:(GetSlotIdsCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK getSlotIds:] API is deprecated. Please use the +[NetflixCloudSave getSlotIds:] API instead.");

+ (void)readSlot:(NSString *)slotId callback:(ReadSlotCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK readSlot:callback:] API is deprecated. Please use the +[NetflixCloudSave readSlot:callback:] API instead.");

+ (void)saveSlot:(NSString *)slotId slotInfo:(SlotInfo *)slotInfo callback:(SaveSlotCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK saveSlot:slotInfo:callback:] API is deprecated. Please use the +[NetflixCloudSave saveSlot:slotInfo:callback:] API instead.");

+ (void)deleteSlot:(NSString *)slotId callback:(DeleteSlotCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK deleteSlot:callback:] API is deprecated. Please use the +[NetflixCloudSave deleteSlot:callback:] API instead.");

+ (void)resolveConflict:(NSString *)slotId resolution:(NGPCloudSaveResolution)resolution callback:(ResolveConflictCallback)callback NGDP_API_DEPRECATED_WITH_MESSAGE("The +[NetflixSDK resolveConflict:resolution:callback:] API is deprecated. Please use the +[NetflixCloudSave resolveConflict:resolution:callback:] API instead.");

@end

NS_ASSUME_NONNULL_END
