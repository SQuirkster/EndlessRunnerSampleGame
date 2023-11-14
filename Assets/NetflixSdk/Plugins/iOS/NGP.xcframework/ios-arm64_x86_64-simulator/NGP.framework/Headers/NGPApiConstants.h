//
//  NGPApiConstants.h
//  NGP
//
//  Created by Allan Zhou on 12/9/21.
//  Copyright (c) 2021 Netflix, Inc.  All rights reserved.
//

#ifndef NGPApiConstants_h
#define NGPApiConstants_h

#define NGDP_API_DEPRECATED __attribute__((deprecated))
#define NGDP_API_DEPRECATED_WITH_MESSAGE(msg) __attribute__((deprecated(msg)))
#import <Foundation/Foundation.h>

typedef enum : int {
    STATUS_CODE_OK = 0,
    // 1 to 999 reserved for per-feature status codes

    // Common error codes
    
    STATUS_CODE_ERROR_UNKNOWN = 1000,
    STATUS_CODE_ERROR_START = STATUS_CODE_ERROR_UNKNOWN,
    STATUS_CODE_ERROR_NOT_FOUND = 1001,
    STATUS_CODE_ERROR_SLOT_LIMIT_EXCEEDED = 1002,
    STATUS_CODE_ERROR_NETWORK = 1003,
    STATUS_CODE_ERROR_PLATFORM_NOT_INITIALIZED = 1004,
    STATUS_CODE_ERROR_USER_PROFILE_NOT_SELECTED = 1005,
    STATUS_CODE_ERROR_INTERRUPTED_BY_PROFILE_SWITCH = 1006,
    STATUS_CODE_ERROR_SIZE_LIMIT_EXCEEDED = 1007,
    STATUS_CODE_ERROR_IO = 1008,
    STATUS_CODE_ERROR_INTERNAL = 1009,
    STATUS_CODE_ERROR_VALIDATION = 1010,
    STATUS_CODE_ERROR_TIMED_OUT = 1011,
    STATUS_CODE_ERROR_UNAVAILABLE = 1012,

    // per-feature codes
    STATUS_CODE_ERROR_CUSTOM_START = 2000,
    STATUS_CODE_ERROR_ENTRY_NOT_FOUND = STATUS_CODE_ERROR_CUSTOM_START,
} NetflixApiStatusCode;

typedef enum : int {
    NGPLocationTopLeft NGDP_API_DEPRECATED = 1,
    NGPLocationTopRight NGDP_API_DEPRECATED = 2,
    NGPLocationBottomLeft NGDP_API_DEPRECATED = 3,
    NGPLocationBottomRight NGDP_API_DEPRECATED = 4,
    NGPLocationCenterRight NGDP_API_DEPRECATED = 5
} NGPLocation NGDP_API_DEPRECATED;

typedef enum : int {
    // push message has been received by the device.
    MessagingEventTypePushNotificationReceived = 0,
    // push notification has been shown to the user.
    MessagingEventTypePushNotificationPresented = 1,
    // user has taken an action on the push notification e.g. clicked on the notification.
    MessagingEventTypePushNotificationAcknowledged = 2,
    // push notification is cleared explicitly by the user.
    MessagingEventTypePushNotificationDismissed = 3,
    // NOT used currently.
    MessagingEventTypePushNotificationIgnored = 4,
    // local notification is scheduled by the user
    MessagingEventTypeLocalNotificationScheduled = 5,
} MessagingEventType;

//TODO: Since public, NetflixCloudSaveStatus for 1.0
typedef enum : int {
    OK = STATUS_CODE_OK,
    SLOT_CONFLICT = 1,

    ERROR_START = STATUS_CODE_ERROR_START,
    ERROR_UNKNOWN = STATUS_CODE_ERROR_UNKNOWN,
    ERROR_UNKNOWN_SLOT_ID = STATUS_CODE_ERROR_NOT_FOUND,
    ERROR_SLOT_LIMIT_EXCEEDED = STATUS_CODE_ERROR_SLOT_LIMIT_EXCEEDED,
    ERROR_NETWORK = STATUS_CODE_ERROR_NETWORK,
    ERROR_PLATFORM_NOT_INITIALIZED = STATUS_CODE_ERROR_PLATFORM_NOT_INITIALIZED,
    ERROR_USER_PROFILE_NOT_SELECTED = STATUS_CODE_ERROR_USER_PROFILE_NOT_SELECTED,
    ERROR_INTERRUPTED_BY_PROFILE_SWITCH = STATUS_CODE_ERROR_INTERRUPTED_BY_PROFILE_SWITCH,
    ERROR_SIZE_LIMIT_EXCEEDED = STATUS_CODE_ERROR_SIZE_LIMIT_EXCEEDED,
    ERROR_IO = STATUS_CODE_ERROR_IO,
    ERROR_INTERNAL = STATUS_CODE_ERROR_INTERNAL,
    ERROR_VALIDATION = STATUS_CODE_ERROR_VALIDATION,
} NGPCloudSaveStatus;

//TODO: Since public, NetflixCloudSaveResolution for 1.0
typedef enum : int {
    KEEP_LOCAL,
    KEEP_REMOTE,
} NGPCloudSaveResolution;

//Map to previously defined
typedef enum : int {
    SUBMIT_STAT_OK = STATUS_CODE_OK,
    SUBMIT_STAT_ERROR_UNKNOWN = STATUS_CODE_ERROR_UNKNOWN,
    SUBMIT_STAT_ERROR_NETWORK = STATUS_CODE_ERROR_NETWORK,
    SUBMIT_STAT_ERROR_PLATFORM_NOT_INITIALIZED = STATUS_CODE_ERROR_PLATFORM_NOT_INITIALIZED,
    SUBMIT_STAT_ERROR_USER_PROFILE_NOT_SELECTED = STATUS_CODE_ERROR_USER_PROFILE_NOT_SELECTED,
    SUBMIT_STAT_ERROR_INTERRUPTED_BY_PROFILE_SWITCH = STATUS_CODE_ERROR_INTERRUPTED_BY_PROFILE_SWITCH,
    SUBMIT_STAT_ERROR_INTERNAL = STATUS_CODE_ERROR_INTERNAL,
    SUBMIT_STAT_ERROR_TIMED_OUT = STATUS_CODE_ERROR_TIMED_OUT,
    SUBMIT_STAT_ERROR_UNAVAILABLE = STATUS_CODE_ERROR_UNAVAILABLE,
    SUBMIT_STAT_ERROR_UNKNOWN_STAT = STATUS_CODE_ERROR_ENTRY_NOT_FOUND,
    SUBMIT_STAT_ERROR_STAT_ARCHIVED = SUBMIT_STAT_ERROR_UNKNOWN_STAT + 1
} NetflixStatsStatus;

//Map to previously defined
typedef enum : int {
    LEADERBOARD_STATUS_OK = STATUS_CODE_OK,
    /**
     * An unexpected error occurred
     */
    LEADERBOARD_STATUS_ERROR_UNKNOWN = STATUS_CODE_ERROR_UNKNOWN,

    /**
     * Error due to network. Caller can retry the operation at a later time
     */
    LEADERBOARD_STATUS_ERROR_NETWORK = STATUS_CODE_ERROR_NETWORK,

    /**
     * Error returned when Leaderboard API call is made before platform is initialized
     */
    LEADERBOARD_STATUS_ERROR_PLATFORM_NOT_INITIALIZED = STATUS_CODE_ERROR_PLATFORM_NOT_INITIALIZED,

    /**
     * Error returned when Leaderboard API call is made without a user profile selected
     */
    LEADERBOARD_STATUS_ERROR_USER_PROFILE_NOT_SELECTED = STATUS_CODE_ERROR_USER_PROFILE_NOT_SELECTED,

    /**
     * Error returned when an operation is interrupted by a profile switch. Caller
     * can retry the operation upon returning to the corresponding profile
     */
    LEADERBOARD_STATUS_ERROR_INTERRUPTED_BY_PROFILE_SWITCH = STATUS_CODE_ERROR_INTERRUPTED_BY_PROFILE_SWITCH,

    /**
     * Error due to IO operations
     */
    LEADERBOARD_STATUS_ERROR_IO = STATUS_CODE_ERROR_IO,

    /**
     * Error due to client-server interactions
     */
    LEADERBOARD_STATUS_ERROR_INTERNAL = STATUS_CODE_ERROR_INTERNAL,
 
    /**
     * Error due to LeaderboardName failing string validation checks
     */
    LEADERBOARD_STATUS_ERROR_VALIDATION = STATUS_CODE_ERROR_VALIDATION,

    /**
     * Leaderboard entry with the given name not found
     */
    LEADERBOARD_STATUS_ERROR_UNKNOWN_LEADERBOARD = STATUS_CODE_ERROR_NOT_FOUND,
    
    /**
     * Leaderboard entry for the current user not found
     */
    LEADERBOARD_STATUS_ERROR_ENTRY_NOT_FOUND = STATUS_CODE_ERROR_ENTRY_NOT_FOUND

} NetflixLeaderboardStatus;

// todo change to netflix fetch direction

typedef NS_ENUM(NSInteger, NetflixFetchDirection) {
    NetflixFetchDirectionBefore = 0,
    NetflixFetchDirectionAfter = 1
} NS_SWIFT_NAME(FetchDirection);

//NetflixFetchDirection?

#endif //NGPApiConstants_h
