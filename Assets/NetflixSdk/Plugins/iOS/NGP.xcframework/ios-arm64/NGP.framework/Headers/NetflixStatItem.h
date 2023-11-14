//
//  NetflixStatItem.h
//  NGP
//
//  Created by Rob Harris on 8/4/22.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface NetflixStatItem : NSObject

@property (nonatomic, readonly) NSString *name;
@property (nonatomic, readonly) NSInteger value;
@property (nullable, nonatomic, readonly) NSString *idempotentKey;

+ (instancetype)new NS_UNAVAILABLE;
- (instancetype)init NS_UNAVAILABLE;

- (instancetype)initWithName:(NSString *)name value:(NSInteger)value;
- (instancetype)initWithName:(NSString *)name
                       value:(NSInteger)value
               idempotentKey:(NSString *)idempotentKey;
@end

NS_ASSUME_NONNULL_END
