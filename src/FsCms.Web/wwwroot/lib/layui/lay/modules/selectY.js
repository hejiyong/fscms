layui.define('layer',function (exports) {
    var $ = layui.$, layer = layui.layer;
    'use strict';
    var P = function(o){
        this.c = {
            //input type = hidden 的元素
            elem:'',
            data:'',
            url:'',
            placeholder:'请选择：',
            disabledTips:'',
            success:function(){}
            //separator: '&nbsp;/&nbsp;'
        };
        this.c = $.extend(this.c,o);
        this.getData = function(url){
            var res;
            $.ajax({
                url:url,
                type: "get",
                dataType:'json',
                async:false,
                success:function(e){
                    res = e.data || layer.msg('数据接口错误，请正确配置');
                },
                error: function(){
                    layer.msg('数据接口错误，请正确配置');
                    res = '';
                }
            });
            return res;
        };
        //生成树
        // this.toTree = function(data,pid){
        //     var o = this, tree = [], temp;
        //     for (var i = 0; i < data.length; i++) {
        //         if (data[i].pid == pid) {
        //             var obj = data[i];
        //             temp = o.toTree(data, data[i].id);
        //             if (temp.length > 0) {
        //                 obj.children = temp;
        //             }
        //             tree.push(obj);
        //         }
        //     }
        //     return tree;
        // };
        this.getChild = function(data,pid){
            var child = [];
            for (var i = 0; i < data.length; i++) {
                if (data[i].pid == pid) {
                    child.push(data[i]);
                }
            }
            return child;
        };
        this.createChild = function(items){
            var o = this;
            var ul = '<ul>';
            for(var i = 0; i < items.length; i++){
                var row = o.getChild(o.data,items[i].id);
                row = row.length != 0 ? 'child-row' : '';
                var off = items[i].off ? 'layui-disabled' : '';
                ul += '<li class ="'+row+' '+off+'" data-id="'+items[i].id+'">'+items[i].name+'</li>'
            }
            ul += '</ul>';
            return ul;
        };
        this.setValue = function (value) {
            var o = this, data = o.data, itemID = [];
            for(var i=0;i<data.length;i++){
                for(var j=0;j<value.length;j++){
                    if(value[j] == data[i].name){
                        //data[i].checked = true;
                        itemID.push(data[i].id);
                    }
                }
            }
            return itemID;
        };
        this.showPopup = function (data,id) {
            var o =this;
            var items = o.getChild(data,id);
            if(items.length != 0){
                var itemsHtml = o.createChild(items);
                $(o.c.elem).parent().find('.pop').append(itemsHtml);
            }
        };
        this.scroll = function () {
            var o = this, c = o.c, ele = c.elem;
            var x = $(ele).parent().find('.pop ul');
            for(var j = 0; j < x.length; j ++){
                var e = x.eq(j).children('li.active');
                if (e[0]) {
                    var t = e.position().top, i = x.eq(j).height(), a = e.height();
                    t > i && x.eq(j).scrollTop(t + x.eq(j).scrollTop() - i + a + 3),
                    t < 0 && x.eq(j).scrollTop(t + x.eq(j).scrollTop() - 5)
                }
            }
        };
        this.data = this.c.url ? this.getData(this.c.url) : this.c.data;
    }
    P.prototype.render = function(){
        var o = this, c = o.c, e = c.elem;
        var html = '<div class="selectY-box"></div>',
            show = '<span class="show"><span class="cl-exp">'+c.placeholder+'</span></span>',
            pop = '<div class="pop"></div>';
        $(e).wrap(html);
        $(e).after(show + pop);

        o.showPopup(o.data,0);

        if($(e).val()!=''){
            var value = $(e).val().split('/');
            var showValue = value.join('<i>/</i>');
            $(e).parent().find('.show').html(showValue);

            var setID = this.setValue(value);
            for(var i = 0; i < setID.length; i ++){
                o.showPopup(o.data,setID[i]);
                var fistUi = $(e).parent().find('.pop ul');
                fistUi.eq(i).find('li[data-id='+setID[i]+']').addClass('active');

            }
        }

        $(e).parent().find('.pop').on('click','ul li',function (en) {
            if($(this).hasClass("layui-disabled")){
                layer.msg(c.disabledTips);
                return false;
            }
            $(this).addClass('active').siblings().removeClass('active');
            $(this).parent().nextAll('ul').remove();
            var id = $(this).data('id');
            o.showPopup(o.data,id);
            if(!$(this).hasClass("child-row")){
                $(e).parent().find('.pop').hide();
                var activeLi = $(e).parent().find('.pop li.active');
                var checkValue = [], ids = [];
                for(var i = 0; i < activeLi.length; i ++){
                    checkValue.push(activeLi.eq(i).html());
                    ids.push(activeLi.eq(i).data('id'))
                }
                var inputValue = checkValue.join('/');
                var showValue =  checkValue.join('<i>/</i>');
                $(e).val(inputValue);
                $(e).parent().find('.show').html(showValue);

                //回调函数
                if (typeof c.success === "function") {
                    var callback = {
                        //数据name 如安徽省/合肥市/蜀山区
                        data:inputValue,
                        //数据ID数组,如1,8,16
                        ids:ids
                    };
                    c.success(callback);
                }
            }
            layui.stope(en);
        });

        $(e).parent().find('.show').on('click',function(en){
            //if()
            $(e).parent().find('.pop').show();
            o.scroll();
            layui.stope(en);
        })
        $(document).on('click',function(){
            $(e).parent().find('.pop').hide();
        })


    }
    exports('selectY', function(o){
         var t =  new P(o);
         t.render();
    });
});