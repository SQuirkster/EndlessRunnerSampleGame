//
//  CNGP.h
//  NGPUI
//
//  Created by Caleb on 5/6/21.
//

#ifndef lib_h
#define lib_h

#include <stdbool.h>
#include <NGP/NGPApiConstants.h>

#if __cplusplus
extern "C" {
#endif

typedef void (*NGPEventDispatcherFuncType)(const char *event);
typedef void (*NGPConfigDispatcherFuncType)(const char *event);
typedef void (*NGPCloudSaveDispatcherFuncType)(const char *event);
typedef void (*NGPCurrentPlayerDispatcherFuncType)(const char *event);
typedef void (*NGPGetPlayerIdentitiesDispatcherFuncType)(const char *event);
typedef void (*NGPLeaderboardCallbackFuncType)(long correlation_id, const char *result);
typedef void (*NGPUnlockAchievementCallbackFuncType)(long correlation_id, const char *result);
typedef void (*NGPGetAchievementsCallbackFuncType)(long correlation_id, const char *result);

void _ngp_set_event_dispatcher(NGPEventDispatcherFuncType dispatcher);

void _ngp_set_config_dispatcher(NGPConfigDispatcherFuncType dispatcher);

void _ngp_set_cloud_save_dispatcher(NGPCloudSaveDispatcherFuncType dispatcher);

void _ngp_test(void);

void ngp_check_user_authentication(void);

void ngp_hide_netflix_menu(void);

void ngp_on_game_state_saved(const char *state);

void ngp_set_locale(const char *locale);

void ngp_show_netflix_menu(NGPLocation location);

void ngp_show_netflix_access_button(void);

void ngp_hide_netflix_access_button(void);

//Only 64bit iOS clients are supported
void ngp_submit_stat(const char *stat_name, const long long stat_value);

void ngp_submit_stat_now(int correlation_id, const char *stat_name, const long long stat_value, const NGPLeaderboardCallbackFuncType callback);

void ngp_get_aggregated_stat(int correlation_id, const char *stat_name, const NGPLeaderboardCallbackFuncType callback);

void ngp_get_top_leaderboard_entries(int correlation_id, const char *leaderboard_name, const int maxEntries, const NGPLeaderboardCallbackFuncType callback);

void ngp_get_more_leaderboard_entries(int correlation_id, const char *leaderboard_name, const int maxEntries, const char *cursor, const NetflixFetchDirection direction,  const NGPLeaderboardCallbackFuncType callback);
    
void ngp_get_entries_around_current_player(int correlation_id, const char *leaderboard_name, const int maxEntries, const NGPLeaderboardCallbackFuncType callback);

void ngp_get_current_player_entry(int correlation_id, const char *leaderboard_name, const NGPLeaderboardCallbackFuncType callback);

void ngp_get_leaderboard_info(int correlation_id, const char *leaderboard_name, const NGPLeaderboardCallbackFuncType callback);

void ngp_unlock_achievement(int correlation_id, const char *achievement_name, const NGPUnlockAchievementCallbackFuncType callback);

void ngp_get_achievements(int correlation_id, NGPGetAchievementsCallbackFuncType callback);

void ngp_show_achievements_panel(void);

void ngp_on_push_token(const char *token_c_string);

void ngp_on_messaging_event(MessagingEventType eventType, const char *json_string);

void ngp_on_deeplink_received(const char *deeplink_url, bool processed_by_game);

void ngp_send_cl_event(const char *cl_type_name, const char *event_data_json);

void ngp_get_slot_ids(int tracker);

void ngp_read_slot(int tracker, const char *slotId);

void ngp_save_slot(int tracker, const char *slotId, void *data, int len);

void ngp_delete_slot(int tracker, const char *slotId);

void ngp_resolve_conflict(int tracker, const char *slotId, NGPCloudSaveResolution resolution);

void ngp_current_player_identity(NGPCurrentPlayerDispatcherFuncType dispatcher);

void ngp_get_player_identities(int tracker, const char *ids, NGPGetPlayerIdentitiesDispatcherFuncType dispatcher);

#if __cplusplus
}   // Extern C
#endif


#endif /* lib_h */
