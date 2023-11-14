//
//  NetflixCloudSave.h
//  NGP
//
//  Created by Eric Warmenhoven on 2/25/22.
//

#import <Foundation/Foundation.h>
#import <NGP/NGPApiConstants.h>
#import <NGP/NGPApiTypes.h>

NS_ASSUME_NONNULL_BEGIN

@interface NetflixCloudSave : NSObject

+ (void)getSlotIds:(GetSlotIdsCallback)callback;

+ (void)readSlot:(NSString *)slotId callback:(ReadSlotCallback)callback;

+ (void)saveSlot:(NSString *)slotId slotInfo:(SlotInfo *)slotData callback:(SaveSlotCallback)callback;

+ (void)deleteSlot:(NSString *)slotId callback:(DeleteSlotCallback)callback;

+ (void)resolveConflict:(NSString *)slotId resolution:(NGPCloudSaveResolution)resolution callback:(ResolveConflictCallback)callback;

@end

NS_ASSUME_NONNULL_END
