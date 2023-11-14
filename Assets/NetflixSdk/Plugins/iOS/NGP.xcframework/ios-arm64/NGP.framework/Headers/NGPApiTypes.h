//
//  NGPApiTypes.h
//  NGP
//
//  Created by Eric Warmenhoven on 3/21/22.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@class NetflixLeaderboardEntriesResult;
@class NetflixLeaderboardEntryResult;
@class NetflixLeaderboardInfoResult;

@interface SlotInfo : NSObject
@property (nonatomic, strong) NSData *data;
@property (nonatomic, nullable, strong) NSString *serverSyncTimestamp;
@end

@interface ConflictResolution : NSObject
@property (nonatomic, strong) SlotInfo *remote;
@property (nonatomic, strong) SlotInfo *local;
@end

@interface CloudSaveCallbackResult : NSObject
@property NGPCloudSaveStatus status;
@property (nullable, nonatomic, strong) NSString *errorDescription;
@end

@interface GetSlotIdsResult : CloudSaveCallbackResult
@property (nonatomic, strong) NSArray<NSString*> *slotIds;
@end

@interface ReadSlotResult : CloudSaveCallbackResult
@property (nullable, nonatomic, strong) SlotInfo *slotInfo;
@property (nullable, nonatomic, strong) ConflictResolution *conflictResolution;
@end

@interface DeleteSlotResult : CloudSaveCallbackResult
@property (nullable, nonatomic, strong) ConflictResolution *conflictResolution;
@end

@interface SaveSlotResult : DeleteSlotResult
@end

@interface ResolveConflictResult : CloudSaveCallbackResult
@end

typedef void (^GetSlotIdsCallback)(GetSlotIdsResult* _Nonnull result);

typedef void (^ReadSlotCallback)(ReadSlotResult* _Nonnull result);

typedef void (^SaveSlotCallback)(SaveSlotResult* _Nonnull result);

typedef void (^DeleteSlotCallback)(DeleteSlotResult* _Nonnull result);

typedef void (^ResolveConflictCallback)(ResolveConflictResult* _Nonnull result);

typedef void (^NetflixLeaderboardEntriesCallback)(NetflixLeaderboardEntriesResult* _Nonnull result);
typedef void (^NetflixLeaderboardEntryCallback)(NetflixLeaderboardEntryResult* _Nonnull result);
typedef void (^NetflixLeaderboardInfoCallback)(NetflixLeaderboardInfoResult* _Nonnull result);


NS_ASSUME_NONNULL_END
