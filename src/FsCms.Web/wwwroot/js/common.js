(function (window) {
    window.base = function () { };
    base.prototype = {
        //全屏loading方法 obj = { msg: "",shade :0, shadeClose:false }
        showLoading: function (obj) {
            var index = layer.msg(obj.msg, {
                icon: 16,
                shade: 0,
                shadeClose: false,
            });
            return index;
        },
        //关闭等待
        closeLoading: function (index) {
            layer.close(index);
        },
        //弹出提示框 options = { msg:"", yes:function(){}, type:1 }
        showMessage: function (options) {
            if (layer == null) {
                alert(options.msg);
                return;
            }
            var yes = function (index) {
                if ($.isFunction(options.yes)) {
                    options.yes();
                }
                layer.close(index);
            };
            layer.alert(options.msg || "操作成功", {
                icon: options.type || 1,
                scrollbar: false,
                shadeClose: false,
                closeBtn: 0,
                skin: 'layui-layer-lan'//'layer-ext-moon'
            }, yes);
        },
        //options={title:"标题",msg:"内容",yes:function,no:function}
        showConfirm: function (options) {
            if (options == null || options.msg == null) {
                return;
            }
            var yes = options.yes;
            var no = options.no;
            var defaultAction = function (index) {
                layer.close(index);
            };
            if (yes == null) {
                yes = defaultAction;
            }
            if (no == null) {
                no = defaultAction
            }
            ////layer.confirm(options.msg, yes, options.title, no);
            //layer.confirm(options.msg, { btn: ['确定', '取消'] }, yes, no);
            layer.confirm(options.msg, {
                btn: ['确定', '取消'], //按钮
                icon: 3,
                shadeClose: false,
                skin: 'layer-ext-moon'
            }, yes, no);
        },
        //Markdown编辑器注册
        markDownEdit: function (id, option) {
            var _option = $.extend({
                width: "96%",
                height: 640,
                syncScrolling: "single",
                path: "../../lib/editormd/lib/"
            }, options);
            return editormd(id, _option);
        },
        //Ajax请求
        ajax: function (url, appendPostData, beforeFn, completeFn, successFn, errorFn, isShowLoading) {
            jQuery.ajax({
                type: "POST",
                url: url,
                data: appendPostData,
                global: false,
                beforeSend: function (XMLHttpRequest) {
                    if (jQuery.isFunction(beforeFn)) {
                        if (beforeFn(XMLHttpRequest)) {
                            if (isShowLoading != false) {
                                freejs.showLoading();
                            }
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        if (isShowLoading != false) {
                            freejs.showLoading();
                        }
                    }
                },
                success: function (data, textStatus) {
                    if (jQuery.isFunction(successFn)) {
                        successFn(data, textStatus);
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    var gohome = XMLHttpRequest.getResponseHeader("Timeout");
                    if (gohome) {
                        // window.top.window.location.href = gohome;
                        return false;
                    }
                    if (isShowLoading != false) {
                        freejs.hideLoading();
                    }
                    if (jQuery.isFunction(completeFn)) {
                        completeFn();
                    }
                },
                error: function (e, d, s, u, b) {
                    if (jQuery.isFunction(errorFn)) {
                        errorFn(e, d, s);
                    }
                    else {
                        freejs.showMessage({
                            title: "发生异常",
                            type: 2,
                            msg: s
                        });
                    }
                }
            });
        },
        //Ajax请求
        ajaxFn: function (options) {
            //url, appendPostData, beforeFn, completeFn, successFn, errorFn, isShowLoading
            var base_options = {
                url: "",
                appendPostData: null,
                beforeFn: null,
                completeFn: null,
                successFn: null,
                errorFn: null,
                isShowLoading: false
            };
            base_options = $.extend(base_options, options);
            jQuery.ajax({
                type: "POST",
                url: base_options.url,
                data: base_options.appendPostData,
                global: false,
                beforeSend: function (XMLHttpRequest) {
                    if (jQuery.isFunction(base_options.beforeFn)) {
                        if (base_options.beforeFn(XMLHttpRequest)) {
                            if (isShowLoading != false) {
                                freejs.showLoading();
                            }
                        }
                        else {
                            return false;
                        }
                    }
                    else {
                        if (isShowLoading != false) {
                            freejs.showLoading();
                        }
                    }
                },
                success: function (data, textStatus) {
                    if (jQuery.isFunction(base_options.successFn)) {
                        base_options.successFn(data, textStatus);
                    }
                },
                complete: function (XMLHttpRequest, textStatus) {
                    var gohome = XMLHttpRequest.getResponseHeader("Timeout");
                    if (gohome) {
                        // window.top.window.location.href = gohome;
                        return false;
                    }
                    if (isShowLoading != false) {
                        freejs.hideLoading();
                    }
                    if (jQuery.isFunction(base_options.completeFn)) {
                        base_options.completeFn();
                    }
                },
                error: function (e, d, s, u, b) {
                    if (jQuery.isFunction(base_options.errorFn)) {
                        base_options.errorFn(e, d, s);
                    }
                    else {
                        freejs.showMessage({
                            title: "发生异常",
                            type: 2,
                            msg: s
                        });
                    }
                }
            });
        },
        //页面级弹出编辑窗口
        dialogWindow: {
            /*
                url: "/Admin/Document/DocContentEditModule",   //页面地址
                paramters: { id: "" },                         //参数
                -------------------------------------------------------
                title: "新增文档",                             //标题
                area: ['1100px', '660px'],                     //尺寸
                submit: {                                      //提交参数
                    url: "/Admin/Document/DocContentCreate",   //   提交的地址
                },                                             //
                callback: reloadTable                          //执行完成回调函数
             */
            create: function (options, formpage) {
                $("#" + options.elmid).load(options.url, options.paramters, function (responseText, textStatus, jqXHR) {
                    switch (textStatus) {
                        case "success":
                            freejs.dialogWindow.open($.extend({
                                type: 1,
                                maxmin: true,
                                title: "编辑",
                                area: ['1100px', '660px'],
                                shadeClose: false, //点击遮罩关闭
                                content: responseText,
                                submit: {
                                    url: "/Admin/Document/DocContentCreate",
                                }
                            }, options), formpage);
                            break;
                        case "error":
                            //freejs.showMessage({ title: "提示", msg: "页面加载失败", type: 2 });
                            freejs.dialogWindow.open($.extend({
                                type: 1,
                                maxmin: true,
                                title: "编辑【失败】",
                                area: ['1100px', '660px'],
                                shadeClose: false, //点击遮罩关闭
                                content: responseText,
                                submit: {
                                    url: "/Admin/Document/DocContentCreate",
                                }
                            }, options), formpage);
                            break;
                    }
                });
                //$.ajax({
                //    type: "GET",
                //    url: options.url,
                //    data: options.paramters,
                //    dataType: "html",
                //    success: function (result) {
                //        console.log(result);
                //    },
                //    error: function (result) {
                //        alert('Failed');
                //    },
                //});
                //$.get(options.url, function () {

                //});
                //ajax获取页面内容，赋值到layui弹出框

            },
            /*
            {
                type: 1,
                maxmin: true,
                title: "编辑",
                area: ['1100px', '660px'],
                shadeClose: false, //点击遮罩关闭
                content: responseText,
                submit: {
                    url: "/Admin/Document/DocContentCreate",
                }
            }
             */
            open: function (options, form) {
                var currentOpenID = 0;
                var base_options = {
                    type: 1,
                    maxmin: true,
                    title: "编辑",
                    area: ['1100px', '660px'],
                    btn: ['立即提交', '关闭'],
                    yes: function (index, layero) {
                        var elmBtn = options.submit.elmBtn || "saveSubmit";
                        form.on('submit(' + elmBtn + ')', function (data) {
                            event.preventDefault();
                            data.field.Id = data.field.Id || 0;
                            if ($.isFunction(options.submitBefore)) data = options.submitBefore(data);
                            var saveLoading = 0;
                            $.ajax({
                                type: 'POST',
                                url: options.submit.url,
                                data: JSON.stringify(data.field),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                beforeSend: function () {
                                    saveLoading = freejs.showLoading({ msg: "数据提交中...", shade: 0.2 });
                                },
                                success: function (e) {
                                    if (e.Status == 1) {
                                        freejs.showMessage({ title: "提示", msg: e.Msg || "保存成功", type: 1 });
                                        if ($.isFunction(options.submitAfter)) options.submitAfter(e);
                                        layer.close(index);
                                    }
                                    else {
                                        freejs.showMessage({ title: "提示", msg: e.Msg, type: 2 });
                                    }
                                    freejs.closeLoading(saveLoading);
                                }
                            });
                            return false;
                        });
                    },
                    //设置窗口关闭事件
                    btn2: function (index, layero) {
                        //layer.confirm('确定要关闭么？', {
                        //    btn: ['确定', '取消'] //按钮
                        //}, function (index, layero) {
                        //    layer.close(index);
                        //    layer.close(currentOpenID);
                        //}, function () {
                        //});
                        layer.close(index);
                        layer.close(currentOpenID);
                        return false;
                    },
                    //设置窗口关闭事件,右上角关闭回调
                    cancel: function (index, layero) {

                        layer.close(index);
                        layer.close(currentOpenID);

                        //layer.confirm('确定要关闭么？', {
                        //    btn: ['确定', '取消'] //按钮
                        //}, function (index, layero) {
                        //    layer.close(index);
                        //    layer.close(currentOpenID);
                        //}, function () {
                        //});
                        return false;
                    },
                    shadeClose: false //点击遮罩关闭
                };
                var new_options = $.extend(base_options, options);
                new_options.success = function (layero, index) {
                    $(".form-module-content").height(new_options.height - 110);

                    // 解决按enter键重复弹窗问题
                    $(':focus').blur();
                    // 添加form标识
                    layero.addClass('layui-form');
                    // 将保存按钮改变成提交按钮
                    layero.find('.layui-layer-btn0').attr({
                        'lay-filter': 'saveSubmit',
                        'lay-submit': ''
                    });
                    if ($.isFunction(options.loadBefore)) options.loadBefore(form);
                    form.render();
                }
                currentOpenID = layer.open(new_options);
            },
            close: function () {
            }
        },
        dataGrid: function (table, form, options) {
            return new dataGrid(table, form, options);
        },
        //顶部tab菜单 fscms_tabsheader
        tabMenu: {
            init: function () {
                //绑定页面tab的基础事件
                freejs.tabMenu.tabEvent();

            },
            getTabCtrl: function () {
                return $("#fscms_tabsheader");
            },
            getCurrentTab: function () {
                return $("#fscms_tabsheader").find(".layui-this").eq(0);
            },
            tabEvent: function (obj) {
                freejs.tabMenu.getTabCtrl().find("li").on('click', function () {
                    var tabHeader = freejs.tabMenu.getTabCtrl();
                    var tempTabs = tabHeader.find("li");
                    var tabId = $(this).attr("tab-id");
                    tempTabs.removeClass("layui-this");
                    $(this).addClass("layui-this");
                    $("#page_content > div").hide();
                    $("#page_content > #page_" + tabId).show();
                });

                freejs.tabMenu.getTabCtrl().find("li .layui-tab-close").unbind("click").bind("click",function () {
                    event.preventDefault();
                    var tabId = $(this).parent().attr("tab-id");
                    $("#page_content").find("#page_" + tabId).eq(0).remove();
                    //显示之前一个页面
                    $(this).parent().prev().addClass("layui-this");
                    var prevTabID = $(this).parent().prev().attr("tab-id");
                    $("#page_content > #page_" + prevTabID).show();
                    $(this).parent().remove();
                    event.stopPropagation();
                });

                $(".closeThisTabs").unbind("click").bind("click",function () {
                    event.preventDefault();
                    var thisTab = freejs.tabMenu.getTabCtrl().find("li.temp_tab.layui-this");
                    if (thisTab) {}
                    var tabId = thisTab.attr("tab-id");
                    $("#page_content").find("#page_" + tabId).eq(0).remove();
                    //显示之前一个页面
                    thisTab.prev().addClass("layui-this");
                    var prevTabID = thisTab.prev().attr("tab-id");
                    $("#page_content > #page_" + prevTabID).show();
                    thisTab.remove();
                    event.stopPropagation();
                });

                $(".closeOtherTabs").unbind("click").bind("click",function () {
                    event.preventDefault();
                    var othertabs = freejs.tabMenu.getTabCtrl().find("li.temp_tab").not("li.temp_tab.layui-this");
                    $.each(othertabs, function (i, item) {
                        var thisTab = $(item);
                        var tabId = thisTab.attr("tab-id");
                        $("#page_content").find("#page_" + tabId).eq(0).remove();
                        thisTab.remove();
                    });
                    event.stopPropagation();
                });

                $(".closeAllTabs").unbind("click").bind("click",function () {
                    event.preventDefault();
                    var othertabs = freejs.tabMenu.getTabCtrl().find("li.temp_tab");
                    $.each(othertabs, function (i, item) {
                        var thisTab = $(item);
                        var tabId = thisTab.attr("tab-id");
                        $("#page_content").find("#page_" + tabId).eq(0).remove();
                        thisTab.remove();
                    });

                    //显示主页
                    var mainTab = freejs.tabMenu.getTabCtrl().find("li.tab_main").eq(0);
                    mainTab.addClass("layui-this");
                    var prevTabID = mainTab.attr("tab-id");
                    $("#page_content > #page_" + prevTabID).show();
                    event.stopPropagation();
                });

            },
            exists: function (pageid) {
                return freejs.tabMenu.getTabCtrl().find("li[tab-id='" + pageid + "']").length > 0;
            },
            add: function (options) {
                //判断节点是否存在
                if (freejs.tabMenu.exists(options.pageid)) {
                    var tabHeader = freejs.tabMenu.getTabCtrl();
                    var tempTabs = tabHeader.find("li");
                    tempTabs.removeClass("layui-this");
                    tabHeader.find("li[tab-id='" + options.pageid + "']").addClass("layui-this");
                    $("#page_content > div").hide();
                    $("#page_content > #page_" + options.pageid).show();
                    return false;
                }
                var ops = {
                    pageid: "0",
                    datajson: "{}",
                    path: "",
                    title: "无标题",
                    ico: ""
                };
                var new_options = $.extend(ops, options);
                //<i class="layui-icon layui-unselect layui-tab-close">ဆ</i>
                var tabHtml = '<li tab-id="'
                    + new_options.pageid
                    + '" datajson="'
                    + new_options.datajson
                    + '" path="'
                    + new_options.path
                    + '" class="temp_tab layui-this"><span>'
                    + new_options.title
                    + '</span><i class="layui-icon layui-unselect layui-tab-close">ဆ</i></li>';
                var tabHeader = freejs.tabMenu.getTabCtrl();
                var tempTabs = tabHeader.find("li");
                if (tempTabs.length > 9) {
                    freejs.showMessage({ msg: "最大支持同时打开10页面，请先关闭部分页面！" });
                    return false;
                }
                tempTabs.removeClass("layui-this");
                tabHeader.append(tabHtml);
                freejs.tabMenu.tabEvent();
                return true;
            },
            delelete: function () {
                //删除
                //移除内容层
            },
            reflash: function () { }
        },
        loadHtml: function (options) {
            $("#" + options.elm).load(options.url, options.paramters, function (responseText, textStatus, jqXHR) {
                freejs.closeLoading(options.loadIndex);
                switch (textStatus) {
                    case "success":
                        if ($.isFunction(options.successCallBack)) options.successCallBack();
                        //初始化绑定页面的时间，例如时间控件
                        index = -1;
                        break;
                    default:
                        if ($.isFunction(options.errorCallBack)) options.errorCallBack(form);
                        $("#" + options.elm).html(responseText || "页面估计不存在");
                        break;
                }
            });
        },
        //页面加载到对应的标签下。
        loadMenu: function (options) {
            //options = {
            //    elm:"page_content",url: "/Admin/Document/DocType", paramters: {},loadIndex:1,isform:false
            //};
            //添加历史打开菜单
            var isCreateOk = freejs.tabMenu.add({
                pageid: options.pageid,
                datajson: JSON.stringify(options.paramters),
                path: options.url,
                title: options.title,
                ico: ""
            });
            //20190507 开启点击菜单重新加载页面
            //if (!isCreateOk) {
            //    freejs.closeLoading(options.loadIndex);
            //    return;
            //}
            var _container = options.elm || "page_content";
            var pageContainer = $("#" + _container);
            if ($("#page_content > #page_" + options.pageid).length == 0) {
                pageContainer.append("<div id='page_" + options.pageid + "'>数据加载中...</div>");
            }
            $("#page_content > div").hide();
            $("#page_content > #page_" + options.pageid).show();
            $("#page_" + options.pageid).load(options.url, options.paramters, function (responseText, textStatus, jqXHR) {
                freejs.closeLoading(options.loadIndex);
                switch (textStatus) {
                    case "success":
                        if ($.isFunction(options.successCallBack)) options.successCallBack();
                        //初始化绑定页面的时间，例如时间控件
                        index = -1;
                        break;
                    //case "notmodified":
                    //case "error":
                    //case "timeout":
                    //case "parsererror":
                    //freesql.loadHtml(mcid, "/Service/Error/", function (jElement, responseText) {
                    //});
                    //break;
                    default:
                        if ($.isFunction(options.errorCallBack)) options.errorCallBack(form);
                        $("#page_" + options.pageid).html(responseText || "页面估计不存在");
                        break;
                }
            });
            //如果出现长时间未关闭，定时关闭loading
            setTimeout(function () {
                if (options.loadIndex >= 0) freejs.closeLoading(options.loadIndex);
            }, 5000);
        }
    };
    /**
     * 数据列表
     * @param {any} table  数据表对象
     * @param {any} form   当前表单对象
     * @param {any} options  列表参数辅助
     *    controller： /Area/Controller
     *    pageElem：当前页面区域对象
     *    elem：列表对象的ID
     *    formElem：表单编辑对象
     *    formArea：表单区域的参数 例如： 默认：['1100px', '660px'] 或者  {edit:['1100px', '660px'], add:['800px', '660px']}
     *    tableOptions：列表的参数
     *          where：查询设置的参数方法
     *          cols：字段配置，详细参考示例
     *          page：是否启动分页
     *          height：列表的高度
     *    editBefore：页面弹出之前渲染的方法
     *    editAfter：页面弹出之后渲染的方法
     *    submitBefore：弹出编辑页面提交之前方法
     *    submitAfter：弹出编辑页面提交之后方法
     *    addParamterExt：新增的时候给新增方法传递的参数
     */
    var dataGrid = function (table, form, options) {
        var baseOptions = {
            elem: "",
            controller: "",
            pageElem: null,
            formElem: null,
            editBefore: null,
            editAfter: null,
            submitBefore: null,
            submitAfter: null,
            addParamterExt: null
        };
        baseOptions = $.extend(baseOptions, options);
        var tableid = baseOptions.elem.replace("#", "");
        var that = this;
        var render = function () {
            var tableOptions = {
                elem: baseOptions.elem,
                url: baseOptions.controller + '/List',
                where: null,
                id: tableid,
                parseData: function (res) { //res 即为原始返回的数据
                    if (res != null && res.rows.length > 0) {
                        return {
                            "code": 0, //解析接口状态
                            "msg": "", //解析提示文本
                            "count": res.records, //解析数据长度
                            "data": res.rows //解析数据列表
                        };
                    }
                    else {
                        return {
                            "code": 0, //解析接口状态
                            "msg": "未获取到任何数据", //解析提示文本
                            "count": 0, //解析数据长度
                            "data": [] //解析数据列表
                        };
                    }
                },
                cols: null,
                page: true,
                height: 400,
            };
            var newOptions = $.extend(tableOptions, options.tableOptions);
            //方法级渲染
            table.render(newOptions);
        };
        //刷新搜索
        this.reload = function () {
            //执行重载 'ArticleDataList'
            table.reload(tableid, {
                page: {
                    curr: 1 //重新从第 1 页开始
                }, where: baseOptions.tableOptions.where()
            });
        };
        //删除
        var deletePost = function (id, callback) {
            //向服务端发送删除指令
            var saveLoading = 0;
            $.ajax({
                type: 'POST',
                url: baseOptions.controller + "/Delete",
                data: { id: id },
                //contentType: "application/json; charset=utf-8",
                //dataType: "json",
                beforeSend: function () {
                    saveLoading = freejs.showLoading({ msg: "数据删除中...", shade: 0.2 });
                },
                success: function (e) {
                    if (e.Status == 1) {
                        freejs.showMessage({ title: "提示", msg: e.Msg || "删除成功", type: 1 });
                        //layer.close(index);
                        callback();
                    }
                    else {
                        freejs.showMessage({ title: "提示", msg: e.Msg, type: 2 });
                    }
                    freejs.closeLoading(saveLoading);
                }
            });
        }
        this.active = null;
        var tableEvent = function () {
            //表格事件
            table.on('row(' + tableid + ')', function (obj) {
                console.log("row：");
                console.log(obj)
            });

            //监听表格行点击
            table.on('tr', function (obj) {
                console.log("监听表格行点击：");
                console.log(obj)
            });

            //监听工具条
            table.on('tool(' + tableid + ')', function (obj) { //注：tool是工具条事件名，test是table原始容器的属性 lay-filter="对应的值"
                event.stopPropagation();
                var data = obj.data; //获得当前行数据
                var layEvent = obj.event; //获得 lay-event 对应的值（也可以是表头的 event 参数对应的值）
                var tr = obj.tr; //获得当前行 tr 的DOM对象
                console.log(obj.tr);
                if (layEvent === 'detail') { //查看

                } else if (layEvent === 'del') { //删除
                    layer.confirm('你确定要删除吗？', function (index) {
                        obj.del(); //删除对应行（tr）的DOM结构，并更新缓存
                        layer.close(index);
                        deletePost(data.Id, function () {
                            if ($.isFunction(baseOptions.submitAfter)) {
                                baseOptions.submitAfter();
                            }
                        });
                    });
                } else if (layEvent === 'edit') { //编辑
                    var options = {
                        url: baseOptions.controller + "/UpdateModule", paramters: { id: data.Id },
                        title: "修改",
                        area: baseOptions.formArea.edit || baseOptions.formArea,
                        submit: {
                            url: baseOptions.controller + "/Update",
                        },
                        elmid: baseOptions.formElem,// "DocContentEdit",
                        loadBefore: function (_form) {
                            baseOptions.editBefore(_form, data);
                            ////监听指定开关
                            //form.on('switch(switchTest)', function (data) {
                            //    if (this.checked) {
                            //        $("#OriginUrlArea", index_ArticleDataList).hide();
                            //        $("#DocContentArea", index_ArticleDataList).show();
                            //    }
                            //    else {
                            //        $("#OriginUrlArea", index_ArticleDataList).show();
                            //        $("#DocContentArea", index_ArticleDataList).hide();

                            //    }
                            //});
                            //loadMarkDown();
                        },
                        submitBefore: function (data) {
                            if ($.isFunction(baseOptions.submitBefore)) {
                                baseOptions.submitBefore(data);
                            }
                            return data;
                        },
                        submitAfter: function (e) {
                            //同步更新缓存对应的值
                            obj.update(e.Data);
                            if ($.isFunction(baseOptions.submitAfter)) {
                                baseOptions.submitAfter(e);
                            }
                        }
                    };
                    freejs.dialogWindow.create(options, form);
                }
                else {
                    if ($.isFunction(baseOptions.rowTool.toolEventExt)) baseOptions.rowTool.toolEventExt(obj, layEvent);
                }
            });
            var $ = layui.$;
            //自定义工具栏事件
            that.active = {
                reload: function () {
                    var seniorQueryJson = {};
                    table.reload(tableid, {
                        page: {
                            curr: 1 //重新从第 1 页开始
                        }, where: baseOptions.tableOptions.where()
                    });
                },
                add: function () {
                    var paramters = { id: "" };
                    if ($.isFunction(baseOptions.addParamterExt)) {
                        paramters = baseOptions.addParamterExt();
                    }
                    var options = {
                        url: baseOptions.controller + "/CreateModule", paramters: paramters,
                        title: "新增",
                        area: baseOptions.formArea.add || baseOptions.formArea,//['1100px', '660px'],
                        submit: {
                            url: baseOptions.controller + "/Create",
                        },
                        elmid: baseOptions.formElem,
                        loadBefore: function () {
                            //监听指定开关
                            form.on('switch(switchTest)', function (data) {
                                if (this.checked) {
                                    $("#OriginUrlArea").hide();
                                    $("#DocContentArea").show();
                                }
                                else {
                                    $("#OriginUrlArea").show();
                                    $("#DocContentArea").hide();

                                }
                            });
                            //loadMarkDown();
                            baseOptions.editBefore(form);
                        },
                        submitBefore: function (data) {
                            data.field.OriginType = data.field.OriginType == "on" ? 1 : 0;
                            if (baseOptions.submitBefore && $.isFunction(baseOptions.submitBefore)) {
                                data = baseOptions.submitBefore(data);
                            }
                            return data;
                        },
                        submitAfter: function () {
                            that.reload();
                            if ($.isFunction(baseOptions.submitAfter)) {
                                baseOptions.submitAfter();
                            }
                        }
                    };
                    freejs.dialogWindow.create(options, form);
                },
                delete: function () {
                    var checkStatus = table.checkStatus(tableid);
                    var data = checkStatus.data;
                    if (data.length == 0) {
                        freejs.showMessage({ title: "提示", msg: "请选择数据行！", type: 2 });
                    }
                    var ids = data.select(s => s.Id).join(',');
                    deletePost(ids, function () {
                        if ($.isFunction(baseOptions.submitAfter)) {
                            baseOptions.submitAfter(e);
                        }
                    });
                },
                update: function () {
                    var checkStatus = table.checkStatus(tableid)
                    var data = checkStatus.data;

                    if (data.length == 0) {
                        freejs.showMessage({ title: "提示", msg: "请选择数据行！", type: 2 });
                    }
                    if (data.length > 1) {
                        freejs.showMessage({ title: "提示", msg: "修改只允许选择一条数据！", type: 2 });
                    }
                    var options = {
                        url: baseOptions.controller + "/UpdateModule", paramters: { id: data[0].Id },
                        title: "修改文档",
                        area: baseOptions.formArea.edit || baseOptions.formArea,//['1100px', '660px'],
                        submit: {
                            url: baseOptions.controller + "/Update",
                        },
                        elmid: baseOptions.formElem,
                        loadBefore: function () {
                            //监听指定开关
                            form.on('switch(switchTest)', function (data) {
                                if (this.checked) {
                                    $("#OriginUrlArea", index_ArticleDataList).hide();
                                    $("#DocContentArea", index_ArticleDataList).show();
                                }
                                else {
                                    $("#OriginUrlArea", index_ArticleDataList).show();
                                    $("#DocContentArea", index_ArticleDataList).hide();

                                }
                            });
                            //loadMarkDown();
                            baseOptions.editBefore(form);
                        },
                        submitBefore: function (data) {
                            data.field.OriginType = data.field.OriginType == "on" ? 1 : 0;
                            return data;
                        },
                        submitAfter: function (e) {
                            //同步更新缓存对应的值
                            obj.update(e.Data);
                            if ($.isFunction(baseOptions.submitAfter)) {
                                baseOptions.submitAfter(e);
                            }
                        }
                    };
                    freejs.dialogWindow.create(options, form);
                },
                export: function () { alert(4); }
            };

            $('.dataGrid_Toolbar .layui-btn', baseOptions.pageElem).on('click', function () {
                event.stopPropagation();
                var type = $(this).data('type');
                that.active[type] ? that.active[type].call(this) : '';
            });
        };
        var init = function () {
            render();
            that.reload();
            tableEvent();
        };
        init();
    };

    window.freejs = new base();

    /**
     * 数组扩展
     * @param {any} func
     */
    Array.prototype.select = function (func) {
        var retValues = [];
        if (this.length == 0) {
            return retValues;
        }
        if (func == null) {
            return this;
        }
        for (var i = 0; i < this.length; i++) {
            retValues.push(func(this[i]));
        }
        return retValues;
    };
    Array.prototype.where = function (func) {
        if (func == null) {
            return this;
        }
        var retList = [];
        for (var i = 0; i < this.length; i++) {
            if (func(this[i]) != false) {
                retList.push(this[i]);
            }
        }
        return retList;
    }
})(window);