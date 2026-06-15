select
    PlanFlag--工单类型 (ZP01国内制品，ZP02日本向,ZP08中间材)
    ,ProductCode as cd--*商品CD
    ,[ZuoFan] as no  --*作番、工单号
    ,ProduceOrder as [shunwei]
    ,amount as suu   --数量
    ,DealerAbbreviation as [jxs_name] --经销商简称
    ,WorkLineCode as [line_cd]--*
    , CONVERT(date,PlanDate,120) [yotei_chk_date] --*预定检查日
    ,H as [h]
    ,W as [w]
    ,DH as [dh]
    ,DW as [dw]
    ,SW as [sw]
    ,FW as [kw] --框宽
    ,b2bOderNo  --B2B订单号
    ,b2bIndexNo --B2B订单序号
    ,BianPlanID
from TCM_BianPlan