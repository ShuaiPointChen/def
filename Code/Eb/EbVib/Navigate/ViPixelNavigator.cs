using System;
using System.Collections.Generic;

class ViPixelNavigateDestChecker_0 : ViAstarDestChecker
{
    //-------------------------------------------------------------------------
    public override bool IsDest(ViAstarStep step)
    {
        return (Object.ReferenceEquals(step, Dest));
    }
    public ViAstarStep Dest;
}

class ViPixelNavigateDestChecker_1 : ViAstarDestChecker
{
    //-------------------------------------------------------------------------
    public override bool IsDest(ViAstarStep step)
    {
        return (ViAstarStep.Distance(step, Dest) < Diff);
    }
    public ViAstarStep Dest;
    public float Diff;
}

public struct ViPixelNavigatePickData
{
    //-------------------------------------------------------------------------
    public Int32 blockX;
    public Int32 blockY;
    public Int32 lastFreeX;
    public Int32 lastFreeY;
}

public class ViPixelNavigator
{
    //-------------------------------------------------------------------------
    Int32 _sizeX;
    Int32 _sizeY;
    ViAstarStep[] _steps;
    List<ViVector3> _route = new List<ViVector3>();
    List<ViAstarStep> _stepPool = new List<ViAstarStep>();
    ViAstar<ViPixelNavigateDestChecker_0> _astar0;
    ViAstar<ViPixelNavigateDestChecker_1> _astar1;
    public List<ViVector3> Route { get { return _route; } }
    public Int32 SizeX { get { return _sizeX; } }
    public Int32 SizeY { get { return _sizeY; } }

    //-------------------------------------------------------------------------
    public ViPixelNavigator(UInt32 heapSize)
    {
        _astar0 = new ViAstar<ViPixelNavigateDestChecker_0>(heapSize);
        _astar1 = new ViAstar<ViPixelNavigateDestChecker_1>(heapSize);
        _astar0.DestChecker = new ViPixelNavigateDestChecker_0();
        _astar1.DestChecker = new ViPixelNavigateDestChecker_1();
    }

    //-------------------------------------------------------------------------
    public void BlockStart(Int32 sizeX, Int32 sizeY)
    {
        Int32 cnt = sizeX * sizeY;
        if (cnt == 0)
        {
            return;
        }
        _steps = new ViAstarStep[cnt];
        _sizeX = sizeX;
        _sizeY = sizeY;

        for (Int32 yIdx = 0; yIdx < _sizeY; ++yIdx)
        {
            for (Int32 xIdx = 0; xIdx < _sizeX; ++xIdx)
            {
                ViAstarStep step = new ViAstarStep();
                step.Cost = 0;
                step.Pos.x = xIdx;
                step.Pos.y = yIdx;
                _steps[xIdx + yIdx * _sizeX] = step;
            }
        }
    }

    //-------------------------------------------------------------------------
    public void SetBlock(Int32 x, Int32 y)
    {
        if (x < _sizeX && y < _sizeY)
        {
            _steps[x + y * _sizeX].Cost = 1.0f;
        }
    }

    //-------------------------------------------------------------------------
    public void BlockEnd()
    {
        for (Int32 yIdx = 1; yIdx < _sizeY - 1; ++yIdx)
        {
            for (Int32 xIdx = 1; xIdx < _sizeX - 1; ++xIdx)
            {
                _Fresh(xIdx, yIdx);
            }
        }
    }

    //-------------------------------------------------------------------------
    public bool IsBlock(Int32 x, Int32 y)
    {
        return IsValue(x, y, 1.0F);
    }

    //-------------------------------------------------------------------------
    public bool IsValue(Int32 x, Int32 y, float fCost)
    {
        if (x < _sizeX && y < _sizeY)
        {
            return (_steps[x + y * _sizeX].Cost == fCost);
        }
        else
        {
            return false;
        }
    }

    //-------------------------------------------------------------------------
    public bool GetRound(Int32 iRootX, Int32 iRootY, float fCost, Int32 iRange, ref Int32 iRoundX, ref Int32 iRoundY)
    {
        if (IsValue(iRootX, iRootY, fCost))
        {
            iRoundX = iRootX;
            iRoundY = iRootY;
            return true;
        }
        for (Int32 iRangeIdx = 1; iRangeIdx < iRange; ++iRangeIdx)
        {
            // iSrcX - iRangeIdx
            for (int iIdx = -iRangeIdx; iIdx <= iRangeIdx; ++iIdx)
            {
                Int32 iX = iRootX - iRangeIdx;
                Int32 iY = iRootY + iIdx;
                if (IsValue(iX, iY, fCost))
                {
                    iRoundX = iX;
                    iRoundY = iY;
                    return true;
                }
            }
            // iSrcX + iRangeIdx
            for (int iIdx = -iRangeIdx; iIdx <= iRangeIdx; ++iIdx)
            {
                Int32 iX = iRootX + iRangeIdx;
                Int32 iY = iRootY + iIdx;
                if (IsValue(iX, iY, fCost))
                {
                    iRoundX = iX;
                    iRoundY = iY;
                    return true;
                }
            }
            // iSrcY - iRangeIdx
            for (int iIdx = -iRangeIdx; iIdx <= iRangeIdx; ++iIdx)
            {
                Int32 iX = iRootX + iIdx;
                Int32 iY = iRootY - iRangeIdx;
                if (IsValue(iX, iY, fCost))
                {
                    iRoundX = iX;
                    iRoundY = iY;
                    return true;
                }
            }
            // iSrcY + iRangeIdx
            for (int iIdx = -iRangeIdx; iIdx <= iRangeIdx; ++iIdx)
            {
                Int32 iX = iRootX + iIdx;
                Int32 iY = iRootY + iRangeIdx;
                if (IsValue(iX, iY, fCost))
                {
                    iRoundX = iX;
                    iRoundY = iY;
                    return true;
                }
            }
        }
        return false;
    }

    //-------------------------------------------------------------------------
    public bool GetRoundInRange1(Int32 iRootX, Int32 iRootY, float fCost, ref Int32 iRoundX, ref Int32 iRoundY)
    {
        if (IsValue(iRootX, iRootY, fCost))
        {
            iRoundX = iRootX;
            iRoundY = iRootY;
            return true;
        }
        for (Int32 iY = iRootY - 1; iY <= iRootY + 1; ++iY)
        {
            for (Int32 iX = iRootX - 1; iX <= iRootX + 1; ++iX)
            {
                if (IsValue(iX, iY, fCost))
                {
                    iRoundX = iX;
                    iRoundY = iY;
                    return true;
                }
            }
        }
        return false;
    }

    //-------------------------------------------------------------------------
    public bool Search(Int32 srcX, Int32 srcY, Int32 destX, Int32 destY, UInt32 maxStep)
    {
        _route.Clear();
        //ViDebuger.Note("Search((" + srcX + ", " + srcY + "), (" + destX + ", " + destY + "))");
        if ((srcX < _sizeX && srcY < _sizeY) == false || (destX < _sizeX && destY < _sizeY) == false)
        {
            return false;
        }
        GetRound(srcX, srcY, 0.0f, 100, ref srcX, ref srcY);
        if (srcX == destX && srcY == destY)
        {
            _route.Add(new ViVector3(srcX, srcY, 0));
            _route.Add(new ViVector3(destX, destY, 0));
            return true;
        }
        if (!Pick(srcX, srcY, destX, destY))
        {
            _route.Add(new ViVector3(srcX, srcY, 0));
            _route.Add(new ViVector3(destX, destY, 0));
            return true;
        }
        ViAstarStep srcStep = _steps[srcX + srcY * _sizeX];
        ViAstarStep destStep = _steps[destX + destY * _sizeX];
        _astar0.DestChecker.Dest = destStep;
        _astar0.MaxStepCnt = maxStep;
        bool reachDest = _astar0.Search(srcStep, destStep);
        _stepPool.Clear();
        _astar0.MakeRoute(_stepPool);
        _astar0.Reset();
        ClipRoute(_stepPool, _route);
        return reachDest;
    }

    //-------------------------------------------------------------------------
    public bool Search(Int32 srcX, Int32 srcY, Int32 destX, Int32 destY, float fDiff, UInt32 maxStep)
    {
        //ViDebuger.Note("Search((" + srcX + ", " + srcY + "), (" + destX + ", " + destY + "))");
        if ((srcX < _sizeX && srcY < _sizeY) == false || (destX < _sizeX && destY < _sizeY) == false)
        {
            return false;
        }
        GetRound(srcX, srcY, 0.0f, 100, ref srcX, ref srcY);
        ViAstarStep srcStep = _steps[srcX + srcY * _sizeX];
        ViAstarStep destStep = _steps[destX + destY * _sizeX];
        _astar1.DestChecker.Dest = destStep;
        _astar1.DestChecker.Diff = fDiff;
        _astar1.MaxStepCnt = maxStep;
        bool reachDest = _astar1.Search(srcStep, destStep);
        _route.Clear();
        _stepPool.Clear();
        _astar1.MakeRoute(_stepPool);
        _astar1.Reset();
        ClipRoute(_stepPool, _route);
        return reachDest;
    }

    //-------------------------------------------------------------------------
    public void ClipRoute(List<ViAstarStep> complex, List<ViVector3> simple)
    {
        if (complex.Count <= 2)
        {
            for (Int32 idx = 0; idx < complex.Count; ++idx)
            {
                ViAstarStep pre = complex[idx];
                simple.Add(new ViVector3(pre.Pos.x, pre.Pos.y, 0));
            }
            return;
        }
        ViAstarStep from = complex[0];
        simple.Add(new ViVector3(from.Pos.x, from.Pos.y, 0));
        for (Int32 idx = 1; idx < complex.Count; ++idx)
        {
            ViAstarStep pre = complex[idx - 1];
            ViAstarStep current = complex[idx];
            if (Pick(from.Pos.x, from.Pos.y, current.Pos.x, current.Pos.y))
            {
                simple.Add(new ViVector3(pre.Pos.x, pre.Pos.y, 0));
                from = pre;
            }
        }
        ViAstarStep back = complex[complex.Count - 1];
        simple.Add(new ViVector3(back.Pos.x, back.Pos.y, 0));
    }

    //-------------------------------------------------------------------------
    public bool Pick(Int32 fromX, Int32 fromY, Int32 destX, Int32 destY)
    {
        ViPixelNavigatePickData data = new ViPixelNavigatePickData();
        return Pick(fromX, fromY, destX, destY, 1.0f, ref data);
    }

    //-------------------------------------------------------------------------
    public bool Pick(Int32 fromX, Int32 fromY, Int32 destX, Int32 destY, float value, ref ViPixelNavigatePickData result)
    {
        Int32 deltaX = destX - fromX;
        Int32 deltaY = destY - fromY;
        if (deltaX == 0 && deltaY == 0)
        {
            return false;
        }
        float absDeltaX = Math.Abs((float)deltaX);
        float absDeltaY = Math.Abs((float)deltaY);
        if (absDeltaX >= absDeltaY)
        {
            if (deltaX < 0)
            {
                return _PickXNegative(fromX, fromY, destX, destY, value, ref result);
            }
            else
            {
                return _PickXPositive(fromX, fromY, destX, destY, value, ref result);
            }
        }
        else
        {
            if (deltaY < 0)
            {
                return _PickYNegative(fromX, fromY, destX, destY, value, ref result);
            }
            else
            {
                return _PickYPositive(fromX, fromY, destX, destY, value, ref result);
            }
        }
    }

    //-------------------------------------------------------------------------
    bool _PickXNegative(Int32 fromX, Int32 fromY, Int32 destX, Int32 destY, float value, ref ViPixelNavigatePickData result)
    {
        Int32 deltaX = destX - fromX;
        Int32 deltaY = destY - fromY;

        float fromCenterX = (float)fromX + 0.5f;
        float fromCenterY = (float)fromY + 0.5f;

        float fDeltaX = -1.0f;
        float fDeltaY = (float)deltaY / Math.Abs((float)deltaX);

        float pickX = fromCenterX;
        float pickY = fromCenterY;

        for (Int32 idx = fromX; idx > destX; --idx)
        {
            pickX += fDeltaX;
            pickY += fDeltaY;

            Int32 newCellX = (Int32)pickX;
            Int32 newCellY = (Int32)pickY;
            if (_steps[newCellX + newCellY * _sizeX].Cost == value)
            {
                result.blockX = newCellX;
                result.blockY = newCellY;
                result.lastFreeX = (Int32)(pickX - fDeltaX);
                result.lastFreeY = (Int32)(pickY - fDeltaY);
                return true;
            }
        }
        if (_steps[destX + destY * _sizeX].Cost == value)
        {
            result.blockX = destX;
            result.blockY = destY;
            result.lastFreeX = (Int32)(pickX);
            result.lastFreeY = (Int32)(pickY);
            return true;
        }
        return false;
    }

    //-------------------------------------------------------------------------
    bool _PickXPositive(Int32 fromX, Int32 fromY, Int32 destX, Int32 destY, float value, ref ViPixelNavigatePickData result)
    {
        Int32 deltaX = destX - fromX;
        Int32 deltaY = destY - fromY;

        float fromCenterX = (float)fromX + 0.5f;
        float fromCenterY = (float)fromY + 0.5f;

        float fDeltaX = 1.0f;
        float fDeltaY = (float)deltaY / Math.Abs((float)deltaX);

        float pickX = fromCenterX;
        float pickY = fromCenterY;

        for (Int32 idx = fromX; idx < destX; ++idx)
        {
            pickX += fDeltaX;
            pickY += fDeltaY;

            Int32 newCellX = (Int32)pickX;
            Int32 newCellY = (Int32)pickY;
            if (_steps[newCellX + newCellY * _sizeX].Cost == value)
            {
                result.blockX = newCellX;
                result.blockY = newCellY;
                result.lastFreeX = (Int32)(pickX - fDeltaX);
                result.lastFreeY = (Int32)(pickY - fDeltaY);
                return true;
            }
        }
        if (_steps[destX + destY * _sizeX].Cost == value)
        {
            result.blockX = destX;
            result.blockY = destY;
            result.lastFreeX = (Int32)(pickX);
            result.lastFreeY = (Int32)(pickY);
            return true;
        }
        return false;
    }

    //-------------------------------------------------------------------------
    bool _PickYNegative(Int32 fromX, Int32 fromY, Int32 destX, Int32 destY, float value, ref ViPixelNavigatePickData result)
    {
        Int32 deltaX = destX - fromX;
        Int32 deltaY = destY - fromY;

        float fromCenterX = (float)fromX + 0.5f;
        float fromCenterY = (float)fromY + 0.5f;

        float fDeltaX = (float)deltaX / Math.Abs((float)deltaY);
        float fDeltaY = -1.0f;
        float pickX = fromCenterX;
        float pickY = fromCenterY;

        for (Int32 idx = fromY; idx > destY; --idx)
        {
            pickX += fDeltaX;
            pickY += fDeltaY;

            Int32 newCellX = (Int32)pickX;
            Int32 newCellY = (Int32)pickY;
            if (_steps[newCellX + newCellY * _sizeX].Cost == value)
            {
                result.blockX = newCellX;
                result.blockY = newCellY;
                result.lastFreeX = (Int32)(pickX - fDeltaX);
                result.lastFreeY = (Int32)(pickY - fDeltaY);
                return true;
            }
        }
        if (_steps[destX + destY * _sizeX].Cost == value)
        {
            result.blockX = destX;
            result.blockY = destY;
            result.lastFreeX = (Int32)(pickX);
            result.lastFreeY = (Int32)(pickY);
            return true;
        }
        return false;
    }

    //-------------------------------------------------------------------------
    bool _PickYPositive(Int32 fromX, Int32 fromY, Int32 destX, Int32 destY, float value, ref ViPixelNavigatePickData result)
    {
        Int32 deltaX = destX - fromX;
        Int32 deltaY = destY - fromY;

        float fromCenterX = (float)fromX + 0.5f;
        float fromCenterY = (float)fromY + 0.5f;

        float fDeltaX = (float)deltaX / Math.Abs((float)deltaY);
        float fDeltaY = 1.0f;
        float pickX = fromCenterX;
        float pickY = fromCenterY;

        for (Int32 idx = fromY; idx < destY; ++idx)
        {
            pickX += fDeltaX;
            pickY += fDeltaY;

            Int32 newCellX = (Int32)pickX;
            Int32 newCellY = (Int32)pickY;
            if (_steps[newCellX + newCellY * _sizeX].Cost == value)
            {
                result.blockX = newCellX;
                result.blockY = newCellY;
                result.lastFreeX = (Int32)(pickX - fDeltaX);
                result.lastFreeY = (Int32)(pickY - fDeltaY);
                return true;
            }
        }
        if (_steps[destX + destY * _sizeX].Cost == value)
        {
            result.blockX = destX;
            result.blockY = destY;
            result.lastFreeX = (Int32)(pickX);
            result.lastFreeY = (Int32)(pickY);
            return true;
        }
        return false;
    }

    //-------------------------------------------------------------------------
    void _Fresh(Int32 x, Int32 y)
    {
        ViAstarStep step = _steps[x + y * _sizeX];
        if (step.Cost > 0.0f)
        {
            return;
        }
        for (Int32 yIdx = y - 1; yIdx < y + 2; ++yIdx)
        {
            for (Int32 xIdx = x - 1; xIdx < x + 2; ++xIdx)
            {
                if (xIdx == x && yIdx == y)
                {
                    continue;
                }
                ViAstarStep roundStep = _steps[xIdx + yIdx * _sizeX];
                if (roundStep.Cost == 0.0f)
                {
                    ViAstarRoundStep node = new ViAstarRoundStep();
                    node.node = roundStep;
                    //
                    float deltaX = x - xIdx;
                    float deltaY = y - yIdx;
                    node.cost = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
                    step.RoundSteps.Add(node);
                }
                else
                {
                    roundStep.Cost = 1.0f;
                }
            }
        }
    }
}