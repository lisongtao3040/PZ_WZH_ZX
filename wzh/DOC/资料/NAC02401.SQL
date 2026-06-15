-- ****************************************************************************
-- システム名   : 産業品フロント
-- モジュール名 : NAC02401.SQL
-- 処理名       : IF品目マスタ(基本)ワークテーブル更新(差分)
-- 注意事項     : 
--
-- Date        EDP-No   所属/氏名              内   容
-- ----------------------------------------------------------------------------
-- 2023/04/26  P-78989  有馬                   新規作成
-- ****************************************************************************

-- エラーメッセージのワーク変数
DECLARE @error_msg VARCHAR(100)
SET @error_msg = ''


DECLARE  @err_num int

SELECT   @err_num = 0



BEGIN TRANSACTION NAC02401


    DECLARE @MAX_IFLastCooperationDateTime datetime                   --


-- I/F最終連携日時の最大値を取得する
    SELECT @MAX_IFLastCooperationDateTime = MAX(IFLastCooperationDateTime)
            FROM  [if_cooperation].[dbo].[IfWorkMaterialForStandard]

    IF (@@error <> 0)
        BEGIN
            SET @error_msg = 'MAX_IFLastCooperationDateTime取得エラー'
            GOTO END_RTN;
        END

-- I/F最終連携日時の最大値が取得出来ない場合（レコードなし）
    IF @MAX_IFLastCooperationDateTime is null
        BEGIN
             SET @MAX_IFLastCooperationDateTime = '1900-01-01 00:00:00.000'
             PRINT @MAX_IFLastCooperationDateTime
        END

--  I/F最終連携日時以降に更新されたデータを連携する。
    MERGE INTO [if_cooperation].[dbo].[IfWorkMaterialForStandard] AS A
        USING (SELECT * from [sapfront_if_cooperation].[dbo].[IfWorkMaterialForStandard] with(nolock)
               WHERE MaterialHierarchy = '1E01'
               AND   MaterialCategory is not null
               AND   IFLastCooperationDateTime > @MAX_IFLastCooperationDateTime) as B
        ON
        (
               A.MaterialCd        = B.MaterialCd COLLATE Japanese_XJIS_100_CI_AS
        )
        WHEN MATCHED THEN
            UPDATE SET
                A.MaterialType = B.MaterialType
               ,A.ExternalMaterialCd = B.ExternalMaterialCd
               ,A.MaterialClassification = B.MaterialClassification
               ,A.BaseUnit = B.BaseUnit
               ,A.BaseUnitOfNetWeight = B.BaseUnitOfNetWeight
               ,A.BaseUnitOfGrossWeight = B.BaseUnitOfGrossWeight
               ,A.BaseUnitOfWeightUnit = B.BaseUnitOfWeightUnit
               ,A.IsDeleteAtClientLevel = B.IsDeleteAtClientLevel
               ,A.MaterialHierarchy = B.MaterialHierarchy
               ,A.Division = B.Division
               ,A.MaterialCategory = B.MaterialCategory
               ,A.RoundingValueForOrderQuantity = B.RoundingValueForOrderQuantity
               ,A.IsSales = B.IsSales
               ,A.IFLastCooperationDateTime = B.IFLastCooperationDateTime
               ,A.Status = '2'
               ,A.InterfaceId = 'T02781'
               ,A.InterfaceSubNumber = '00'
               ,A.CreatedOn = GETDATE()
               ,A.CreatedFunction = 'PWC_MAM_S001'
               ,A.CreatedBy = ''
               ,A.UpdateOn = GETDATE()
               ,A.UpdateFunction = 'PWC_MAM_S001'
               ,A.UpdateBy = ''
               ,A.IsDelete = B.IsDelete
        WHEN NOT MATCHED THEN
            INSERT
            (
                MaterialCd
               ,MaterialType
               ,ExternalMaterialCd
               ,MaterialClassification
               ,BaseUnit
               ,BaseUnitOfNetWeight
               ,BaseUnitOfGrossWeight
               ,BaseUnitOfWeightUnit
               ,IsDeleteAtClientLevel
               ,MaterialHierarchy
               ,Division
               ,MaterialCategory
               ,RoundingValueForOrderQuantity
               ,IsSales
               ,IFLastCooperationDateTime
               ,Status
               ,InterfaceId
               ,InterfaceSubNumber
               ,CreatedOn
               ,CreatedFunction
               ,CreatedBy
               ,UpdateOn
               ,UpdateFunction
               ,UpdateBy
               ,IsDelete
            )
            VALUES
            (
                B.MaterialCd
               ,B.MaterialType
               ,B.ExternalMaterialCd
               ,B.MaterialClassification
               ,B.BaseUnit
               ,B.BaseUnitOfNetWeight
               ,B.BaseUnitOfGrossWeight
               ,B.BaseUnitOfWeightUnit
               ,B.IsDeleteAtClientLevel
               ,B.MaterialHierarchy
               ,B.Division
               ,B.MaterialCategory
               ,B.RoundingValueForOrderQuantity
               ,B.IsSales
               ,B.IFLastCooperationDateTime
               ,'2'
               ,'T02781'
               ,'00'
               ,GETDATE()
               ,'PWC_MAM_S001'
               ,''
               ,GETDATE()
               ,'PWC_MAM_S001'
               ,''
               ,B.IsDelete
            )
    ;

    IF (@@error <> 0)
        BEGIN
            SET @error_msg = 'INSERTでエラー'
            GOTO END_RTN;
        END

-- 終了処理
END_RTN:
    IF @error_msg = ''
        BEGIN
            COMMIT TRANSACTION NAC02401
                        PRINT 'OSQL Success'
        END
    ELSE
        BEGIN
            ROLLBACK TRANSACTION NAC02401
                        PRINT 'OSQL Failure'
                        PRINT @error_msg
        END

