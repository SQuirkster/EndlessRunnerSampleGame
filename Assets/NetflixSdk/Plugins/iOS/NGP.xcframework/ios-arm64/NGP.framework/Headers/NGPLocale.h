//
//  NGPLocale.h
//  NGP
//
//  Created by Allan Zhou on 12/9/21.
//  Copyright (c) 2021 Netflix, Inc.  All rights reserved.
//

#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN
/**
 *  @class
 *      Locale
 *  @abstract
 *      A simple Locale implementation
 *  @discussion
 *      reference: BCP47 https://tools.ietf.org/search/bcp47
 *      The class name collides with Foundation.Locale, so use `Foundation.Locale` or `NGP.Locale` when you need to distinguish them
 */
@interface Locale : NSObject
/**
 *  @abstract
 *      Represents the locale language
 *  @discussion
 *      Sample values confirm to
 *      - BCP-47 2 char Primary Language Subtag: "en", "fr", "ko", "ja", "it" etc.
 *      - BCP-47 2 char Primary Language Subtag + "-" + BCP-47 4 char Script Subtag: "zh-Hant", "zh-Hans".
 *      BCP-47 2 char Primary Language Subtag uses lower case.
 *      BCP-47 4 char Script Subtag is capitalized (initial letter upper case and others lower case).
 *      3 char Primary Language Subtag is allowed but not recommended.
 */
@property (nonatomic, readonly) NSString *language;
/**
 *  @abstract
 *      Represents the locale country / region
 *  @discussion
 *      Sample value "US", "DE", "VN", "BR" etc. Confirms to BCP-47 2 char Region Subtag.
 *      BCP-47 2 char Region Subtag uses uppercase.
 */
@property (nonatomic, readonly) NSString *country;
/**
 *  @abstract
 *      Represents the locale variant
 *  @discussion
 *      sample value, "", "1901", "pinyin" etc. Confirms to BCP-47 Variant Subtags
 */
@property (nonatomic, readonly) NSString *variant;
/**
 *  @abstract
 *      Create an instance of Locale with language, country and variant
 *  @parameter language
 *      the locale language, Sample values confirm to
 *      - BCP-47 2 char Primary Language Subtag: "en", "fr", "ko", "ja", "it" etc
 *      - BCP-47 2 char Primary Language Subtag + "-" + BCP-47 4 char Script Subtag
 *          "zh-Hant", "zh-Hans".
 *      BCP-47 2 char Primary Language Subtag uses lower case.
 *      BCP-47 4 char Script Subtag uses lower case with the initial letter capitalized.
 *      3 char Primary Language Subtag is allowed but not recommended.
 *  @parameter country
 *      the locale country / region.
 *      Sample values: "US", "DE", "VN", "BR" etc. Confirms to BCP-47 2 char Region Subtag.
 *      BCP-47 2 char Region Subtag uses uppercase.
 *  @parameter variant
 *      the locale variant.
 *      Sample values: "", "1901", "pinyin" etc. Confirms to BCP-47 Variant Subtags
 *  @discussion
 *      This may return nil if length of `language` is shorter than 2, or other invalided cases.
 */
- (nullable instancetype)initWithLanguage:(NSString *)language
                                  country:(NSString *)country
                                  variant:(NSString *)variant;



+ (instancetype)new NS_UNAVAILABLE;
- (instancetype)init NS_UNAVAILABLE;
@end
NS_ASSUME_NONNULL_END
