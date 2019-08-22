/**
 layui官网
*/
layui.define(['form', 'element', 'table', 'util'], function (exports) {
    var $ = layui.jquery
        , element = layui.element
        , layer = layui.layer
        , form = layui.form
        , util = layui.util
        , device = layui.device()
        , $win = $(window), $body = $('body');

    //阻止IE7以下访问
    if (device.ie && device.ie < 8) {
        layer.alert('Layui最低支持ie8，您当前使用的是古老的 IE' + device.ie + '，你丫的肯定不是程序猿！');
    }

    var home = $('#LAY_home');

    layer.ready(function () {
        layui.data('layui', {
            key: 'version', value: layui.v
        });
    });

    //一些事件监听
    form.on('select(component)', function (data) {
        var value = data.value;
        location.href = "/doc?id=" + value;
    });

    !function () {

    }();

    //点击事件
    var events = {
        //联系方式
        contactInfo: function () {
            layer.alert('<div class="layui-text">如有合作意向，可联系：<br>邮箱：hogancode.com</div>', {
                title: '联系'
                , btn: false
                , shadeClose: true
            });
        }
    }

    //$body.on('click', '*[site-event]', function () {
    //    var othis = $(this)
    //        , attrEvent = othis.attr('site-event');
    //    events[attrEvent] && events[attrEvent].call(this, othis);
    //});

    //数字前置补零
    var digit = function (num, length, end) {
        var str = '';
        num = String(num);
        length = length || 2;
        for (var i = num.length; i < length; i++) {
            str += '0';
        }
        return num < Math.pow(10, length) ? str + (num | 0) : num;
    };

    ////获取下载数
    //$.get('#', function (res) {
    //$.get('#', function (res) {
    //    $('.site-showdowns').html(res.number);
    //}, 'jsonp');

    ////记录下载
    //$('.site-down').on('click', function () {
    //    $.get('#', function () { }, 'jsonp');
    //});

    //获取Github数据
    var getStars = $('#getStars');
    var getStars = $('#getFock');
    //var getWatch = $('#getWatch');
    if (getStars[0]) {
        $.get('https://api.github.com/repos/2881099/FreeSql', function (res) {
            getStars.html(res.stargazers_count);
            getStars.html(res.forks_count);
            //getWatch.html(res.watchers_count);
        }, 'json');
    }

    //固定Bar
    //if (global.pageType !== 'demo') {
    //    util.fixbar({
    //        bar1: true
    //        , click: function (type) {
    //            if (type === 'bar1') {
    //                location.href = 'https://github.com/2881099/FreeSql/issues';
    //            }
    //        }
    //    });
    //}

    //窗口scroll
    ; !function () {
        var main = $('.site-tree').parent(), scroll = function () {
            var stop = $(window).scrollTop();

            if ($(window).width() <= 750) return;
            var bottom = $('.footer').offset().top - $(window).height();
            if (stop > 211 && stop < bottom) {
                if (!main.hasClass('site-fix')) {
                    main.addClass('site-fix');
                }
                if (main.hasClass('site-fix-footer')) {
                    main.removeClass('site-fix-footer');
                }
            } else if (stop >= bottom) {
                if (!main.hasClass('site-fix-footer')) {
                    main.addClass('site-fix site-fix-footer');
                }
            } else {
                if (main.hasClass('site-fix')) {
                    main.removeClass('site-fix').removeClass('site-fix-footer');
                }
            }
            stop = null;
        };
        scroll();
        $(window).on('scroll', scroll);
    }();

    //示例页面滚动
    $('.site-demo-body').on('scroll', function () {
        var elemDate = $('.layui-laydate,.layui-colorpicker-main')
            , elemTips = $('.layui-table-tips');
        if (elemDate[0]) {
            elemDate.each(function () {
                var othis = $(this);
                if (!othis.hasClass('layui-laydate-static')) {
                    othis.remove();
                }
            });
            $('input').blur();
        }
        if (elemTips[0]) elemTips.remove();

        if ($('.layui-layer')[0]) {
            layer.closeAll('tips');
        }
    });

    //代码修饰
    //layui.code({
    //    elem: 'pre', 
    //    title: "free.code",
    //    encode: false,
    //    about: false
    //});

    //目录
    var siteDir = $('.site-dir');
    if (siteDir[0] && $(window).width() > 750) {
        layer.ready(function () {
            layer.open({
                type: 1
                , content: siteDir
                , skin: 'layui-layer-dir'
                , area: 'auto'
                , maxHeight: $(window).height() - 300
                , title: '目录'
                //,closeBtn: false
                , offset: 'r'
                , shade: false
                , success: function (layero, index) {
                    layer.style(index, {
                        marginLeft: -15
                    });
                }
            });
        });
        siteDir.find('li').on('click', function () {
            var othis = $(this);
            othis.find('a').addClass('layui-this');
            othis.siblings().find('a').removeClass('layui-this');
        });
    }

    //在textarea焦点处插入字符
    var focusInsert = function (str) {
        var start = this.selectionStart
            , end = this.selectionEnd
            , offset = start + str.length

        this.value = this.value.substring(0, start) + str + this.value.substring(end);
        this.setSelectionRange(offset, offset);
    };

    //演示页面
    $('body').on('keydown', '#LAY_editor, .site-demo-text', function (e) {
        var key = e.keyCode;
        if (key === 9 && window.getSelection) {
            e.preventDefault();
            focusInsert.call(this, '  ');
        }
    });

    var editor = $('#LAY_editor')
        , iframeElem = $('#LAY_demo')
        , demoForm = $('#LAY_demoForm')[0]
        , demoCodes = $('#LAY_demoCodes')[0]
        , runCodes = function () {
            if (!iframeElem[0]) return;
            var html = editor.val();

            html = html.replace(/=/gi, "layequalsign");
            html = html.replace(/script/gi, "layscrlayipttag");
            demoCodes.value = html.length > 100 * 1000 ? '<h1>卧槽，你的代码过长</h1>' : html;

            demoForm.action = '/api/runHtml/';
            demoForm.submit();

        };
    $('#LAY_demo_run').on('click', runCodes), runCodes();

    //让导航在最佳位置
    var setScrollTop = function (thisItem, elemScroll) {
        if (thisItem[0]) {
            var itemTop = thisItem.offset().top
                , winHeight = $(window).height();
            if (itemTop > winHeight - 120) {
                elemScroll.animate({ 'scrollTop': itemTop / 2 }, 200)
            }
        }
    }
    setScrollTop($('.site-demo-nav').find('dd.layui-this'), $('.layui-side-scroll').eq(0));
    setScrollTop($('.site-demo-table-nav').find('li.layui-this'), $('.layui-side-scroll').eq(1));


    //查看代码
    $(function () {

    });

    //点击查看代码选项
    element.on('tab(demoTitle)', function (obj) {
        if (obj.index === 1) {
            if (device.ie && device.ie < 9) {
                layer.alert('强烈不推荐你通过ie8/9 查看代码！因为，所有的标签都会被格式成大写，且没有换行符，影响阅读');
            }
        }
    })


    //手机设备的简单适配
    var treeMobile = $('.site-tree-mobile')
        , shadeMobile = $('.site-mobile-shade')

    treeMobile.on('click', function () {
        $('body').addClass('site-mobile');
    });

    shadeMobile.on('click', function () {
        $('body').removeClass('site-mobile');
    });
    exports('global', {});
});