using System;
using Model.Dto.Menu;

namespace Interface
{
	public interface IMenuService
	{
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="role"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> Add(MenuAdd req, string userId);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="role"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> Edit(MenuEdit req, string userId);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> Del(string id);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> BatchDel(string ids);
        /// <summary>
        /// 获取带权限的菜单列表 （ 什么角色，就看什么菜单）
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<List<MenuRes>> GetMenus(MenuReq req, string userId);
        /// <summary>
        /// 设置菜单
        /// </summary>
        /// <param name="rid"></param>
        /// <param name="mids"></param>
        /// <returns></returns>
        Task<bool> SettingMenu(string rid, string mids);
    }
}

